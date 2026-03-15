using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SupportChat.Application.Abstractions;
using SupportChat.Infrastructure.Persistence;

namespace SupportChat.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } =
        Path.Combine(Path.GetTempPath(), $"supportchat-test-{Guid.NewGuid():N}.db");

    public CustomWebApplicationFactory()
    {
        IntegrationTestDatabaseRegistry.Register(DatabasePath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<SupportChatDbContext>));
            services.RemoveAll(typeof(SupportChatDbContext));
            services.RemoveAll(typeof(IChatSessionRepository));

            services.AddDbContext<SupportChatDbContext>(options =>
                options.UseSqlite($"Data Source={DatabasePath}"));

            services.AddScoped<IChatSessionRepository, SqliteChatSessionRepository>();

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SupportChatDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }
}