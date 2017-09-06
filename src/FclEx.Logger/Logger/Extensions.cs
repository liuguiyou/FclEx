using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FclEx.Logger
{
    public static class Extensions
    {
        public static ILoggerFactory AddEmpty(this ILoggerFactory factory)
        {
            factory.AddProvider(EmptyLoggerProvider.Instance);
            return factory;
        }

        public static ILoggerFactory AddSimpleConsole(this ILoggerFactory factory)
        {
            factory.AddProvider(new SimpleConsoleLoggerProvider());
            return factory;
        }

        public static void Log(this ILogger logger, string str, LogLevel level = LogLevel.Information)
        {
            switch (level)
            {
                case LogLevel.Trace: logger.LogTrace(str); break;
                case LogLevel.Debug: logger.LogDebug(str); break;
                case LogLevel.Information: logger.LogInformation(str); break;
                case LogLevel.Warning: logger.LogWarning(str); break;
                case LogLevel.Error: logger.LogError(str); break;
                case LogLevel.Critical: logger.LogCritical(str); break;

                case LogLevel.None:
                default:
                    break;
            }
        }

    }
}
