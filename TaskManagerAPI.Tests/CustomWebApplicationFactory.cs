using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskManagerAPI.Controllers;
using TaskManagerAPI.Data;

namespace TaskManagerAPI.Tests;

// Use a public type from the API assembly without changing the API's internal Program.
public class CustomWebApplicationFactory
    : WebApplicationFactory<VehiclesController>
{
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseContentRoot(
            FindApiContentRoot()
        );

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfiguration = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TransportManagerTestsSigningKeyAtLeast32Characters!",
                ["Jwt:Issuer"] = "TransportManagerTests",

                ["Admin:Username"] = "",
                ["Admin:Password"] = ""
            };

            config.AddInMemoryCollection(testConfiguration);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            _connection.Open();

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlite(_connection)
            );

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var database =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            database.Database.EnsureCreated();
        });
    }

    private static string FindApiContentRoot()
    {
        for (
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            directory != null;
            directory = directory.Parent
        )
        {
            var candidate = Path.Combine(
                directory.FullName,
                "TaskManagerAPI"
            );

            var projectFile = Path.Combine(
                candidate,
                "TaskManagerAPI.csproj"
            );

            if (File.Exists(projectFile))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate the API content root."
        );
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}