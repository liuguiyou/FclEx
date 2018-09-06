using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw.Logging
{
    public static class Extensions
    {
        public static ILoggerFactory AddColorConsole(this ILoggerFactory factory, LogLevel minLevel = LogLevel.Information)
        {
            factory.AddProvider(new ColorConsoleLoggerProvider(minLevel));
            return factory;
        }

        public static ILoggingBuilder AddColorConsole(this ILoggingBuilder builder, LogLevel minLevel = LogLevel.Information)
        {
            builder.Services.AddSingleton<ILoggerProvider>(new ColorConsoleLoggerProvider(minLevel));
            return builder;
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
