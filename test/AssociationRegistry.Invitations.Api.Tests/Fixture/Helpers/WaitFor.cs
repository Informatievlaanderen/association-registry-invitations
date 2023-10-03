using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture.Helpers;

public static class WaitFor
{
    public abstract class WaitForBase
    {
        protected static void Wait(ILogger logger, Action waitAction, string serviceName)
        {
            var watch = Stopwatch.StartNew();
            var tryCount = 0;
            var exit = false;
            while (!exit)
            {
                try
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Waiting until {ServiceName} becomes available ... ({WatchElapsed})",
                            serviceName, watch.Elapsed);
                    }

                    waitAction();

                    exit = true;
                }
                catch (Exception exception)
                {
                    if (tryCount >= 20)
                        throw new TimeoutException(
                            $"Service {serviceName} throws exception {exception.Message} after 5 tries", exception);

                    tryCount++;
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(exception,
                            "Encountered an exception while waiting for {ServiceName} to become available",
                            serviceName);
                    }

                    Thread.Sleep(1000 * tryCount);
                }
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("{ServiceName} became available ... ({WatchElapsed})", serviceName,
                    watch.Elapsed);
            }
        }

        protected static async Task WaitAsync(ILogger logger, Func<Task> waitAction, string serviceName,
            CancellationToken cancellationToken = default)
        {
            var watch = Stopwatch.StartNew();
            var tryCount = 0;
            var exit = false;
            while (!exit)
            {
                try
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Waiting until {ServiceName} becomes available ... ({WatchElapsed})",
                            serviceName, watch.Elapsed);
                    }

                    await waitAction();

                    exit = true;
                }
                catch (Exception exception)
                {
                    if (tryCount >= 20)
                        throw new TimeoutException(
                            $"Service {serviceName} throws exception {exception.Message} after 5 tries", exception);

                    tryCount++;
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(exception,
                            "Encountered an exception while waiting for {ServiceName} to become available",
                            serviceName);
                    }

                    await Task.Delay(1000 * tryCount, cancellationToken);
                }
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("{ServiceName} became available ... ({WatchElapsed})", serviceName,
                    watch.Elapsed);
            }
        }
    }

    public class Postgres : WaitForBase
    {
        private static void TryProbePostgres(string connectionString)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            const string cmdText = "SELECT 1 FROM pg_database WHERE datname='postgres'";
            using var cmd = new NpgsqlCommand(cmdText, conn);
            if (cmd.ExecuteScalar() == null)
                throw new Exception("Root Postgres database does not yet exist");
        }

        private static async Task TryProbePostgresAsync(string connectionString)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            const string cmdText = "SELECT 1 FROM pg_database WHERE datname='postgres'";
            await using var cmd = new NpgsqlCommand(cmdText, conn);
            if (cmd.ExecuteScalar() == null)
                throw new Exception("Root Postgres database does not yet exist");
        }

        public static Task ToBecomeAvailableAsync(ILogger logger, string connectionString,
            CancellationToken cancellationToken = default)
            => WaitAsync(logger, () => TryProbePostgresAsync(connectionString), "PostgreSQL", cancellationToken);


        public static void ToBecomeAvailable(ILogger logger, string connectionString)
            => Wait(logger, () => TryProbePostgres(connectionString), "PostgreSQL");
    }
}
