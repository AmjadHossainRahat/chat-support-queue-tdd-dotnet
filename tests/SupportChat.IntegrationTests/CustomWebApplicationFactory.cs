using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportChat.Infrastructure.Persistence;

namespace SupportChat.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath;
    private readonly string _connectionString;

    public CustomWebApplicationFactory()
    {
        _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"supportchat-test-{Guid.NewGuid():N}.db");

        _connectionString = $"Data Source={_databasePath}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SupportChatDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<SupportChatDbContext>(options =>
            {
                options.UseSqlite(_connectionString);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SupportChatDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        SqliteConnection.ClearAllPools();
        DeleteDatabaseFileWithRetry(_databasePath);
    }

    private static void DeleteDatabaseFileWithRetry(string databasePath)
    {
        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (File.Exists(databasePath))
                {
                    File.Delete(databasePath);
                }

                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Task.Delay(100).GetAwaiter().GetResult();
                SqliteConnection.ClearAllPools();
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Task.Delay(100).GetAwaiter().GetResult();
                SqliteConnection.ClearAllPools();
            }
        }
    }
}