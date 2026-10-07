using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests;

internal sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly ServiceProvider _provider;

    public AppDbContext Db { get; }
    public UserManager<ApplicationUser> Users { get; }
    public RoleManager<IdentityRole> Roles { get; }

    public TestDatabase()
    {
        _connection.Open();

        var services = new ServiceCollection();

        services.AddLogging();

        services.AddDbContext<AppDbContext>(
            options => options.UseSqlite(_connection));

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        _provider = services.BuildServiceProvider();

        Db = _provider.GetRequiredService<AppDbContext>();
        Users = _provider.GetRequiredService<UserManager<ApplicationUser>>();
        Roles = _provider.GetRequiredService<RoleManager<IdentityRole>>();

        Db.Database.EnsureCreated();
    }

    public async Task<ApplicationUser> CreateUserAsync(
        string username = "testuser")
    {
        var user = new ApplicationUser
        { 
            UserName = username
        };

        var result = await Users.CreateAsync(
            user,
            "Password123!"
         );

        Assert.True(result.Succeeded);

        return user;
    }

    public async Task CreateRolesAsync()
    {
        var roles = new[]
        { 
          "Admin",
          "Employee",
          "Viewer" 
        };

        foreach (var role in roles)
        {   
            var identityRole = new IdentityRole(role);
            
            var result = await Roles.CreateAsync(identityRole);

            Assert.True(result.Succeeded);
        }
    }

    public void Dispose()
    {
        _provider.Dispose();
        _connection.Dispose();
    }
}
