using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class TaskServiceTests
{
    [Fact]
    public void GetAll_ReturnsOnlyTasksOwnedByUser()
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

        var projectB = new Project
        {
            Name = "Project B",
            UserId = userB.Id
        };

        db.Projects.AddRange(projectA, projectB);
        db.SaveChanges();

        db.Tasks.AddRange(
            new TaskItem
            {
                Title = "Task A",
                ProjectId = projectA.Id,
                IsDone = false
            },
            new TaskItem
            {
                Title = "Task B",
                ProjectId = projectB.Id,
                IsDone = false
            }
        );

        db.SaveChanges();

        var service = new TaskService(db);

        // Act
        var result = service.GetAll(userA.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal("Task A", result[0].Title);
    }

    [Fact]
    public void Create_ReturnsNull_WhenProjectBelongsToAnotherUser()
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

        var dto = new CreateTaskDto
        {
            Title = "Hacked task",
            Description = "Should not be created",
            ProjectId = projectA.Id
        };

        var service = new TaskService(db);

        // Act
        var result = service.Create(dto, userB.Id);

        // Assert
        Assert.Null(result);
        Assert.Empty(db.Tasks);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenTaskBelongsToAnotherUser()
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

        var taskA = new TaskItem
        {
            Title = "Task A",
            ProjectId = projectA.Id,
            IsDone = false
        };

        db.Tasks.Add(taskA);
        db.SaveChanges();

        var service = new TaskService(db);

        // Act
        var result = service.GetById(taskA.Id, userB.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenTaskBelongsToAnotherUser()
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

        var taskA = new TaskItem
        {
            Title = "Task A",
            ProjectId = projectA.Id,
            IsDone = false
        };

        db.Tasks.Add(taskA);
        db.SaveChanges();

        var service = new TaskService(db);

        // Act
        var result = service.Delete(taskA.Id, userB.Id);

        // Assert
        Assert.False(result);
        Assert.Single(db.Tasks);
    }
}