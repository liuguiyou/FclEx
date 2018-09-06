using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw.Logging
{
    public class ColorConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minLevel;
        private readonly ConcurrentDictionary<string, ColorConsoleLogger> _loggers = new ConcurrentDictionary<string, ColorConsoleLogger>();

        public ColorConsoleLoggerProvider(LogLevel minLevel = LogLevel.Information)
        {
            _minLevel = minLevel;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, (name) => new ColorConsoleLogger(name, _minLevel));
        }
    }
}
