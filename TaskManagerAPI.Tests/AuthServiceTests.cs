using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Login_ReturnsUser_WhenCredentialsAreValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);

        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        db.Users.Add(user);
        db.SaveChanges();

        var service = new AuthService(db);

        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "password123"
        };

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }


    [Fact]
    public void Login_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);

        var service = new AuthService(db);

        var dto = new LoginDto
        {
            Username = "nonexistent",
            Password = "password123"
        };

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public void Login_ReturnsNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);

        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        db.Users.Add(user);
        db.SaveChanges();

        var service = new AuthService(db);

        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.Null(result);
    }
}