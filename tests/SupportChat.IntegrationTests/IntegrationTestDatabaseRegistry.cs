using System.Collections.Concurrent;

namespace SupportChat.IntegrationTests;

public static class IntegrationTestDatabaseRegistry
{
    private static readonly ConcurrentBag<string> _databasePaths = new();

    public static void Register(string databasePath)
    {
        _databasePaths.Add(databasePath);
    }

    public static IReadOnlyCollection<string> GetAll()
    {
        return _databasePaths.ToArray();
    }
}