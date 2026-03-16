using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Queues;
using SupportChat.Domain.Sessions;
using SupportChat.Infrastructure.Configuration;
using SupportChat.Infrastructure.Persistence;
using SupportChat.Infrastructure.Providers;

namespace SupportChat.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetRequiredConnectionString("SupportChat");

        services.AddDbContext<SupportChatDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddSingleton(new OfficeHoursPolicy(
            new TimeOnly(9, 0),
            new TimeOnly(17, 0)));

        services.AddSingleton<QueueAdmissionPolicy>(sp =>
        {
            var officeHoursPolicy = sp.GetRequiredService<OfficeHoursPolicy>();
            return new QueueAdmissionPolicy(officeHoursPolicy);
        });

        services.AddSingleton(new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3));

        services.AddScoped<IChatSessionRepository, SqliteChatSessionRepository>();
        services.AddSingleton<IAgentProvider, InMemoryAgentProvider>();

        services.AddSingleton<AssignmentPolicy>();

        services.AddScoped<CreateChatSessionUseCase>();
        services.AddScoped<RegisterPollUseCase>();
        services.AddScoped<AssignWaitingSessionUseCase>();
        services.AddScoped<AssignNextQueuedSessionUseCase>();
        services.AddScoped<GetChatSessionByIdUseCase>();
        services.AddScoped<MarkInactiveSessionUseCase>();

        services.AddScoped<QueuedSessionAssignmentProcessor>();
        services.AddScoped<InactiveSessionProcessor>();

        return services;
    }
}