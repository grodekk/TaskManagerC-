using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskManagerAPI.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task GetProjects_ReturnsUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsToken_AndAllowsAccessToProtectedEndpoint()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Register
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                username = "integrationUser",
                password = "password123"
            });

        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"REGISTER: {registerResponse.StatusCode}");
        Console.WriteLine(registerBody);

        //Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        Assert.True(
        registerResponse.IsSuccessStatusCode,
        $"Register failed with {registerResponse.StatusCode}: {registerBody}"
        );

        // Login
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "integrationUser",
                password = "password123"
            });

        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"LOGIN: {loginResponse.StatusCode}");
        Console.WriteLine(loginBody);


        //Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.True(
        loginResponse.IsSuccessStatusCode,
        $"Login failed with {loginResponse.StatusCode}: {loginBody}"
        );

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = json.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        // Add JWT to following requests
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Protected endpoint
        var projectsResponse = await client.GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, projectsResponse.StatusCode);
    }

    [Fact]
    public async Task GetProject_ReturnsNotFound_WhenProjectBelongsToAnotherUser()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        var clientA = factory.CreateClient();
        var clientB = factory.CreateClient();

        // Register user A
        var registerA = await clientA.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                username = "userA",
                password = "password123"
            });

        Assert.Equal(HttpStatusCode.OK, registerA.StatusCode);

        // Login user A
        var loginA = await clientA.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "userA",
                password = "password123"
            });

        var loginJsonA =
            await loginA.Content.ReadFromJsonAsync<JsonElement>();

        var tokenA = loginJsonA
            .GetProperty("token")
            .GetString();

        clientA.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenA);

        // User A creates project
        var createProject = await clientA.PostAsJsonAsync(
            "/api/projects",
            new
            {
                name = "Project A"
            });

        Assert.Equal(HttpStatusCode.Created, createProject.StatusCode);

        var projectJson =
            await createProject.Content.ReadFromJsonAsync<JsonElement>();

        var projectId =
            projectJson.GetProperty("id").GetInt32();


        // Register user B
        var registerB = await clientB.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                username = "userB",
                password = "password123"
            });

        Assert.Equal(HttpStatusCode.OK, registerB.StatusCode);

        // Login user B
        var loginB = await clientB.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "userB",
                password = "password123"
            });

        var loginJsonB =
            await loginB.Content.ReadFromJsonAsync<JsonElement>();

        var tokenB = loginJsonB
            .GetProperty("token")
            .GetString();

        clientB.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenB);


        // Act
        var response =
            await clientB.GetAsync($"/api/projects/{projectId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}