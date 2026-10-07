using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests;

public class IntegrationTests
{
    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        CustomWebApplicationFactory factory, string? role)
    {
        var clientOptions = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        };

        var client = factory.CreateClient(clientOptions);

        var username = $"user{Guid.NewGuid():N}";
        const string password = "Password123!";

        await RegisterUserAsync(client, username, password);

        if (role is not null)
        {
            await AssignRoleAsync(factory, username, role);
        }

        var token = await LoginAndGetTokenAsync(
            client,
            username,
            password
        );

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static async Task RegisterUserAsync(
        HttpClient client,
        string username,
        string password)
    {
        var credentials = new
        {
            username,
            password
        };

        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            credentials
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task AssignRoleAsync(
        CustomWebApplicationFactory factory,
        string username,
        string role)
    {
        using var scope = factory.Services.CreateScope();

        var users = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var user = await users.FindByNameAsync(username);

        Assert.NotNull(user);

        var result = await users.AddToRoleAsync(user, role);

        Assert.True(result.Succeeded);
    }

    private static async Task<string> LoginAndGetTokenAsync(
        HttpClient client,
        string username,
        string password)
    {
        var credentials = new
        {
            username,
            password
        };

        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            credentials
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResult =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        var token = loginResult
            .GetProperty("token")
            .GetString();

        var hasToken = !string.IsNullOrWhiteSpace(token);

        Assert.True(hasToken);

        return token!;
    }

    private static async Task<int> AddVehicleAsync(
        CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Vehicles.Add(vehicle);
        await database.SaveChangesAsync();

        return vehicle.Id;
    }

    [Fact]
    public async Task GetVehicles_ReturnsForbidden_WhenAuthenticatedUserHasNoRole()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client =
            await CreateAuthenticatedClientAsync(factory, null);

        // Act
        using var response =
            await client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetVehicles_ReturnsOk_ForViewer()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client =
            await CreateAuthenticatedClientAsync(factory, "Viewer");

        var vehicleId = await AddVehicleAsync(factory);

        // Act
        using var response =
            await client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var vehicles =
            await response.Content.ReadFromJsonAsync<List<VehicleDto>>();

        Assert.NotNull(vehicles);

        var vehicle = Assert.Single(vehicles);

        Assert.Equal(vehicleId, vehicle.Id);
    }

    [Fact]
    public async Task PostServiceRecord_ReturnsForbidden_ForViewer()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client =
            await CreateAuthenticatedClientAsync(factory, "Viewer");

        var vehicleId = await AddVehicleAsync(factory);
        var dto = CreateRecord();

        // Act
        using var response = await client.PostAsJsonAsync(
            $"/api/vehicles/{vehicleId}/service-records",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );

        using var scope = factory.Services.CreateScope();

        var database =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Empty(database.ServiceRecords);
        Assert.Empty(database.ServiceRecordItems);
    }

    [Theory]
    [InlineData("Employee")]
    [InlineData("Admin")]
    public async Task PostServiceRecord_ReturnsCreated_AndPersistsRecord(
        string role)
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client =
            await CreateAuthenticatedClientAsync(factory, role);

        var vehicleId = await AddVehicleAsync(factory);
        var dto = CreateRecord();

        // Act
        using var response = await client.PostAsJsonAsync(
            $"/api/vehicles/{vehicleId}/service-records",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        var result =
            await response.Content.ReadFromJsonAsync<ServiceRecordDto>();

        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        Assert.Equal(dto.PerformedOn, result.PerformedOn);
        Assert.Equal(dto.Mileage, result.Mileage);

        var resultItem = Assert.Single(result.Items);

        Assert.Equal("Oil change", resultItem.Description);

        using var scope = factory.Services.CreateScope();

        var database =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var savedRecord = await database.ServiceRecords
            .Include(record => record.Items)
            .SingleAsync();

        Assert.Equal(result.Id, savedRecord.Id);
        Assert.Equal(vehicleId, savedRecord.VehicleId);

        Assert.Equal(dto.PerformedOn, savedRecord.PerformedOn);
        Assert.Equal(dto.Mileage, savedRecord.Mileage);

        var savedItem = Assert.Single(savedRecord.Items);

        Assert.Equal(resultItem.Id, savedItem.Id);
        Assert.Equal("Oil change", savedItem.Description);
    }

    [Theory]
    [InlineData("Viewer")]
    [InlineData("Employee")]
    public async Task Admin_CanAssignAllowedRole_ThroughUsersEndpoint(
        string role)
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();

        using var client =
            await CreateAuthenticatedClientAsync(factory, "Admin");

        using var scope = factory.Services.CreateScope();

        var users =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var targetUser = new ApplicationUser
        {
            UserName = "targetuser"
        };

        var createUserResult = await users.CreateAsync(
            targetUser,
            "Password123!"
        );

        Assert.True(createUserResult.Succeeded);

        var dto = new UpdateUserRoleDto
        {
            Role = role
        };

        // Act
        using var response = await client.PutAsJsonAsync(
            $"/api/users/{targetUser.Id}/role",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var roles = await users.GetRolesAsync(targetUser);
        var assignedRole = Assert.Single(roles);

        Assert.Equal(role, assignedRole);
    }

    private static ServiceRecordCreateDto CreateRecord()
    {
        return new ServiceRecordCreateDto
        {
            PerformedOn = new DateOnly(2026, 9, 1),
            Mileage = 120000,
            Items = new()
            {
                new()
                {
                    Description = "Oil change"
                }
            }
        };
    }
}