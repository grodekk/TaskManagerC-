using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class UserServiceTests
{
    [Theory]
    [InlineData("Employee", "Viewer")]
    [InlineData("Viewer", "Employee")]
    public async Task UpdateRoleAsync_AssignsAllowedRole_AndReplacesPreviousRole(
        string previousRole,
        string newRole)
    {
        // Arrange
        using var database = new TestDatabase();

        await database.CreateRolesAsync();

        var user = await database.CreateUserAsync();

        var assignRoleResult =
            await database.Users.AddToRoleAsync(user, previousRole);

        Assert.True(assignRoleResult.Succeeded);

        var service = new UserService(database.Users, database.Roles);

        // Act
        var result = await service.UpdateRoleAsync(user.Id, newRole);

        // Assert
        Assert.Equal(UpdateRoleResult.Success, result);

        var roles = await database.Users.GetRolesAsync(user);
        var assignedRole = Assert.Single(roles);

        Assert.Equal(newRole, assignedRole);
    }

    [Fact]
    public async Task UpdateRoleAsync_ReturnsUserNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new UserService(database.Users, database.Roles);

        // Act
        var result = await service.UpdateRoleAsync("missing", "Viewer");

        // Assert
        Assert.Equal(UpdateRoleResult.UserNotFound, result);
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("Admin")]
    public async Task UpdateRoleAsync_ReturnsInvalidRole_AndPreservesCurrentRole(string role)
    {
        // Arrange
        using var database = new TestDatabase();

        await database.CreateRolesAsync();

        var user = await database.CreateUserAsync();
        var assignRoleResult = await database.Users.AddToRoleAsync(user, "Viewer");

        Assert.True(assignRoleResult.Succeeded);

        var service = new UserService(database.Users, database.Roles);

        // Act
        var result = await service.UpdateRoleAsync(user.Id, role);

        // Assert
        Assert.Equal(UpdateRoleResult.InvalidRole, result);

        var roles = await database.Users.GetRolesAsync(user);
        var currentRole = Assert.Single(roles);

        Assert.Equal("Viewer", currentRole);
    }

    [Fact]
    public async Task UpdateRoleAsync_ReturnsInvalidRole_WhenAllowedRoleIsMissingFromStore()
    {
        // Arrange
        using var database = new TestDatabase();

        var user = await database.CreateUserAsync();

        var service = new UserService(database.Users, database.Roles);

        // Act
        var result = await service.UpdateRoleAsync(user.Id, "Viewer");

        // Assert
        Assert.Equal(UpdateRoleResult.InvalidRole, result);

        var roles = await database.Users.GetRolesAsync(user);

        Assert.Empty(roles);
    }
}
