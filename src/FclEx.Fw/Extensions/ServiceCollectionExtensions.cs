using System.ComponentModel;
using FclEx.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FclEx.Fw.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Add<T, TImpl>(this IServiceCollection services, ServiceLifetime lifetime)
            where T : class where TImpl : class, T
        {
            Check.NotNull(services, nameof(services));
            services.Add(new ServiceDescriptor(typeof(T), typeof(TImpl), lifetime));
            return services;
        }

        public static IServiceCollection TryAdd<T, TImpl>(this IServiceCollection services, ServiceLifetime lifetime)
            where T : class where TImpl : class, T
        {
            Check.NotNull(services, nameof(services));
            services.TryAdd(new ServiceDescriptor(typeof(T), typeof(TImpl), lifetime));
            return services;
        }
    }
}
