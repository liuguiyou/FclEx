using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fw.Dependency.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static bool TryGetService<T>(this IServiceProvider provider, out T service)
        {
            service = provider.GetService<T>();
            return service != null;
        }

        public static ILogger GetLogger<T>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetLogger(typeof(T));
        }

        public static ILogger GetLogger(this IServiceProvider serviceProvider, Type type)
        {
            var fac = serviceProvider.GetService<ILoggerFactory>();
            if (fac == null)
            {
                return serviceProvider.GetService<ILogger>() ?? NullLogger.Instance;
            }
            else
            {
                var logger = fac.CreateLogger(type);
                return logger;
            }
        }
    }
}
