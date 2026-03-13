using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Queues;
using SupportChat.Infrastructure.Persistence;

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

        services.AddSingleton<CreateChatSessionUseCase>();
        services.AddSingleton<RegisterPollUseCase>();

        return services;
    }
}