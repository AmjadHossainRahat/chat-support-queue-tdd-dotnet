using NUnit.Framework;

namespace SupportChat.IntegrationTests;

[SetUpFixture]
public class IntegrationTestCleanup
{
    [OneTimeTearDown]
    public void CleanupDatabaseFiles()
    {
        foreach (var path in IntegrationTestDatabaseRegistry.GetAll().Distinct())
        {
            TryDeleteWithRetry(path);
        }
    }

    private static void TryDeleteWithRetry(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                File.Delete(path);
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(200);
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Thread.Sleep(200);
            }
        }
    }
}