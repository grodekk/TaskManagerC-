using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class ProjectServiceTests
{
    [Fact]
    public void GetAll_ReturnsOnlyProjectsOwnedByUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);

        var userA = new User
        {
            Username = "userA",
            PasswordHash = "hash"
        };

        var userB = new User
        {
            Username = "userB",
            PasswordHash = "hash"
        };

        db.Users.AddRange(userA, userB);
        db.SaveChanges();

        db.Projects.AddRange(
            new Project
            {
                Name = "Project A",
                UserId = userA.Id
            },
            new Project
            {
                Name = "Project B",
                UserId = userB.Id
            }
        );

        db.SaveChanges();

        var service = new ProjectService(db);

        // Act
        var result = service.GetAll(userA.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal("Project A", result[0].Name);
    }


    [Fact]
    public void GetById_ReturnsNull_WhenProjectBelongsToAnotherUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);

        var userA = new User
        {
            Username = "userA",
            PasswordHash = "hash"
        };

        var userB = new User
        {
            Username = "userB",
            PasswordHash = "hash"
        };

        db.Users.AddRange(userA, userB);
        db.SaveChanges();

        var projectA = new Project
        {
            Name = "Project A",
            UserId = userA.Id
        };

        db.Projects.Add(projectA);
        db.SaveChanges();

        var service = new ProjectService(db);

        // Act
        var result = service.GetById(projectA.Id, userB.Id);

        // Assert
        Assert.Null(result);
    }


}