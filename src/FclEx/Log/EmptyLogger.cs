using System;
using Microsoft.Extensions.Logging;

namespace FclEx.Log
{
    public class EmptyLogger : ILogger
    {
        public static EmptyLogger Instance { get; } = new EmptyLogger();

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
            return EmptyDisposable.Instance;
        }
    }
}
