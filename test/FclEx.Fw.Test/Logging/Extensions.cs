using FclEx.Fw.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FclEx.Fw.Test.Logging
{
    public static class Extensions
    {
        public static ILoggerFactory AddTest(this ILoggerFactory factory, ITestOutputHelper output)
        {
            factory.AddProvider(new TestLoggerProvider(output));
            return factory;
        }

        public static ILoggingBuilder AddTest(this ILoggingBuilder builder, ITestOutputHelper output)
        {
            builder.Services.AddSingleton<ILoggerProvider>(new TestLoggerProvider(output));
            return builder;
        }
    }
}
