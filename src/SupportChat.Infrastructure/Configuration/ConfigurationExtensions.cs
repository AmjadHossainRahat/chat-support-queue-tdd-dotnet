using Microsoft.Extensions.Configuration;

namespace SupportChat.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static string GetRequiredConnectionString(
        this IConfiguration configuration,
        string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' is missing.");
        }

        return connectionString;
    }
}