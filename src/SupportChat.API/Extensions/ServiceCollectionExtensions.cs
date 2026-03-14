using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Queues;
using SupportChat.Infrastructure.Persistence;
using SupportChat.Infrastructure.Providers;

namespace SupportChat.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(new OfficeHoursPolicy(
            new TimeOnly(9, 0),
            new TimeOnly(17, 0)));

        services.AddSingleton<QueueAdmissionPolicy>(sp =>
        {
            var officeHoursPolicy = sp.GetRequiredService<OfficeHoursPolicy>();
            return new QueueAdmissionPolicy(officeHoursPolicy);
        });

        services.AddSingleton<IChatSessionRepository, InMemoryChatSessionRepository>();
        services.AddSingleton<IAgentProvider, InMemoryAgentProvider>();

        services.AddSingleton<AssignmentPolicy>();

        services.AddSingleton<CreateChatSessionUseCase>();
        services.AddSingleton<RegisterPollUseCase>();
        services.AddSingleton<AssignWaitingSessionUseCase>();
        services.AddSingleton<AssignNextQueuedSessionUseCase>();
        services.AddSingleton<QueuedSessionAssignmentProcessor>();

        return services;
    }
}