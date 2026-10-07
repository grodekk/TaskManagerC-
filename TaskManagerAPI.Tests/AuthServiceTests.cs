using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUserWithHashedPassword()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new AuthService(database.Users);

        var dto = new RegisterDto
        {
            Username = "newuser",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.Succeeded);

        var user = await database.Users.FindByNameAsync(dto.Username);
        Assert.NotNull(user);

        var passwordIsValid =
            await database.Users.CheckPasswordAsync(user, dto.Password);

        Assert.True(passwordIsValid);
    }

    [Fact]
    public async Task RegisterAsync_DoesNotAssignAnyRole()
    {
        // Arrange
        using var database = new TestDatabase();

        await database.CreateRolesAsync();

        var service = new AuthService(database.Users);

        var dto = new RegisterDto
        {
            Username = "newuser",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.Succeeded);

        var user = await database.Users.FindByNameAsync(dto.Username);
        Assert.NotNull(user);

        var roles = await database.Users.GetRolesAsync(user);

        Assert.Empty(roles);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUser_WhenCredentialsAreValid()
    {
        // Arrange
        using var database = new TestDatabase();

        var expectedUser = await database.CreateUserAsync();

        var service = new AuthService(database.Users);

        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
    }

    [Theory]
    [InlineData("testuser", "WrongPassword123!")]
    [InlineData("missing", "Password123!")]
    public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid(
        string username,
        string password)
    {
        // Arrange
        using var database = new TestDatabase();

        await database.CreateUserAsync();

        var service = new AuthService(database.Users);

        var dto = new LoginDto
        {
            Username = username,
            Password = password
        };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Null(result);
    }
}