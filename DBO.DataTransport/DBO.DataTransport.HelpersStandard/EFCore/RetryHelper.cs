using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DBO.DataTransport.HelpersStandard.EFCore
{
    public static class RetryHelper
    {
        public const int MaxProcessingAttempts2 = 2;

        private enum SqlExceptionErrorTypes
        {
            Timeout = -2,
            NoLock = 1204,
            Deadlock = 1205,
            WordbreakerTimeout = 30053,
        }

        private static readonly Logger log = LogManager.GetLogger(string.Empty, MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task RetryOnExceptionAsync(uint times, TimeSpan? delay, CancellationToken ct, Func<Task> operation)
        {
            await RetryOnExceptionAsync<Exception>(times, delay, ct, operation);
        }

        public static async Task RetryOnExceptionAsync(TimeSpan? delay, CancellationToken ct, Func<Task> operation)
        {
            await RetryOnExceptionAsync<Exception>(0, delay, ct, operation);
        }

        public static async Task RetryOnExceptionAsync(CancellationToken ct, Func<Task> operation)
        {
            await RetryOnExceptionAsync<Exception>(0, null, ct, operation);
        }

        public static async Task RetryOnExceptionAsync<TException>(uint times, TimeSpan? delay, CancellationToken ct, Func<Task> operation) where TException : Exception
        {
            times = times == 0 ? MaxProcessingAttempts2 : times;
            var attempts = 0;
            do
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    attempts++;
                    await operation();
                    break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (TException ex)
                {
                    if (attempts >= times)
                    {
                        log.Warn($"Exception on attempt {attempts}. Stop retry.");
                        throw;
                    }

                    await CreateDelayForException(attempts, delay, ex);
                }
            } while (true);
        }

        private static Task CreateDelayForException(int attempts, TimeSpan? delay, Exception ex)
        {
            var sleep = (int)(delay?.TotalMilliseconds ?? GetRandomInterval(attempts, ex));
            log.Warn(ex, $"Exception on attempt {attempts}. Retry after sleep {sleep}.");
            return Task.Delay(sleep);
        }

        private static int GetRandomInterval(int attempts, Exception exception)
        {
            int randomIntverval = 1000;
            if (exception is SqlException && ((SqlException)exception).Number == (int)SqlExceptionErrorTypes.Timeout)
                randomIntverval *= attempts * 10;
            else if (attempts <= MaxProcessingAttempts2)
            {
                var random = new Random(1);
                randomIntverval = random.Next(200, 500);
            }
            else
                randomIntverval *= 3;
            return randomIntverval;
        }

    }
}
