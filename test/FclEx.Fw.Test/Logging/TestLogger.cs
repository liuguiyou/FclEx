using System;
using FclEx.Utils;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FclEx.Fw.Test.Logging
{
    public class TestLogger : ILogger
    {
        private readonly ITestOutputHelper _output;

        public TestLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                var msg = exception.IsNull() ? message : message + Environment.NewLine + exception;
                _output.WriteLine(msg);
            }
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
