using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Assignments;
using SupportChat.Infrastructure.Persistence;
using SupportChat.Infrastructure.Providers;
using SupportChat.Worker.Assignment;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SupportChat")
                       ?? "Data Source=supportchat.db";

builder.Services.AddDbContext<SupportChatDbContext>(options =>
    options.UseSqlite(connectionString));

// services.AddSingleton<IChatSessionRepository, InMemoryChatSessionRepository>();
builder.Services.AddScoped<IChatSessionRepository, SqliteChatSessionRepository>();
builder.Services.AddSingleton<IAgentProvider, InMemoryAgentProvider>();

builder.Services.AddSingleton<AssignmentPolicy>();
builder.Services.AddScoped<AssignWaitingSessionUseCase>();
builder.Services.AddScoped<AssignNextQueuedSessionUseCase>();
builder.Services.AddScoped<QueuedSessionAssignmentProcessor>();

builder.Services.AddHostedService<QueuedSessionAssignmentWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SupportChatDbContext>();
    dbContext.Database.EnsureCreated();
}

host.Run();