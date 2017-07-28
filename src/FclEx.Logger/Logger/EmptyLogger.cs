using System;
using Microsoft.Extensions.Logging;

namespace FclEx.Logger
{
    public class EmptyLogger : ILogger
    {
        public static EmptyLogger Logger { get; } = new EmptyLogger();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return Disposable.Singleton;
        }
    }

    internal class Disposable : IDisposable
    {
        public static Disposable Singleton { get; } = new Disposable();

        public void Dispose() { }
    }
}
