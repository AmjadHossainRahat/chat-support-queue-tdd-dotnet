using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Sessions;
using SupportChat.Infrastructure.Persistence;
using SupportChat.Infrastructure.Providers;
using SupportChat.Worker.Assignment;
using SupportChat.Worker.Sessions;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SupportChat")
                       ?? "Data Source=supportchat.db";

builder.Services.AddDbContext<SupportChatDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IChatSessionRepository, SqliteChatSessionRepository>();
builder.Services.AddSingleton<IAgentProvider, InMemoryAgentProvider>();

builder.Services.AddSingleton<AssignmentPolicy>();
builder.Services.AddSingleton(new SessionActivityPolicy(
    expectedPollInterval: TimeSpan.FromSeconds(1),
    missedPollThreshold: 3));

builder.Services.AddScoped<AssignWaitingSessionUseCase>();
builder.Services.AddScoped<AssignNextQueuedSessionUseCase>();
builder.Services.AddScoped<QueuedSessionAssignmentProcessor>();

builder.Services.AddScoped<MarkInactiveSessionUseCase>();
builder.Services.AddScoped<InactiveSessionProcessor>();

builder.Services.AddHostedService<QueuedSessionAssignmentWorker>();
builder.Services.AddHostedService<InactiveSessionMonitorWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SupportChatDbContext>();
    dbContext.Database.EnsureCreated();
}

host.Run();