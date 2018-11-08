using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FclEx.Http.HttpClientExt
{
    internal static class Log
    {
        private static class EventIds
        {
            public static readonly EventId CleanupCycleStart = new EventId(100, "CleanupCycleStart");
            public static readonly EventId CleanupCycleEnd = new EventId(101, "CleanupCycleEnd");
            public static readonly EventId CleanupItemFailed = new EventId(102, "CleanupItemFailed");
            public static readonly EventId HandlerExpired = new EventId(103, "HandlerExpired");
        }

        private static readonly Action<ILogger, int, Exception> _cleanupCycleStart = LoggerMessage.Define<int>(
            LogLevel.Debug,
            EventIds.CleanupCycleStart,
            "Starting HttpMessageHandler cleanup cycle with {InitialCount} items");

        private static readonly Action<ILogger, double, int, int, Exception> _cleanupCycleEnd = LoggerMessage.Define<double, int, int>(
            LogLevel.Debug,
            EventIds.CleanupCycleEnd,
            "Ending HttpMessageHandler cleanup cycle after {ElapsedMilliseconds}ms - processed: {DisposedCount} items - remaining: {RemainingItems} items");

        private static readonly Action<ILogger, string, Exception> _cleanupItemFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.CleanupItemFailed,
            "HttpMessageHandler.Dispose() threw and unhandled exception for client: '{ClientName}'");

        private static readonly Action<ILogger, double, string, Exception> _handlerExpired = LoggerMessage.Define<double, string>(
            LogLevel.Debug,
            EventIds.HandlerExpired,
            "HttpMessageHandler expired after {HandlerLifetime}ms for client '{ClientName}'");


        public static void CleanupCycleStart(ILogger logger, int initialCount)
        {
            _cleanupCycleStart(logger, initialCount, null);
        }

        public static void CleanupCycleEnd(ILogger logger, TimeSpan duration, int disposedCount, int finalCount)
        {
            _cleanupCycleEnd(logger, duration.TotalMilliseconds, disposedCount, finalCount, null);
        }

        public static void CleanupItemFailed(ILogger logger, string clientName, Exception exception)
        {
            _cleanupItemFailed(logger, clientName, exception);
        }

        public static void HandlerExpired(ILogger logger, string clientName, TimeSpan lifetime)
        {
            _handlerExpired(logger, lifetime.TotalMilliseconds, clientName, null);
        }
    }
}
