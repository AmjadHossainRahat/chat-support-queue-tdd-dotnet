using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Assignments;
using SupportChat.Infrastructure.Persistence;
using SupportChat.Infrastructure.Providers;
using SupportChat.Worker.Assignment;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChatSessionRepository, InMemoryChatSessionRepository>();
builder.Services.AddSingleton<IAgentProvider, InMemoryAgentProvider>();

builder.Services.AddSingleton<AssignmentPolicy>();
builder.Services.AddSingleton<AssignWaitingSessionUseCase>();
builder.Services.AddSingleton<AssignNextQueuedSessionUseCase>();
builder.Services.AddSingleton<QueuedSessionAssignmentProcessor>();

builder.Services.AddHostedService<QueuedSessionAssignmentWorker>();

var host = builder.Build();
host.Run();