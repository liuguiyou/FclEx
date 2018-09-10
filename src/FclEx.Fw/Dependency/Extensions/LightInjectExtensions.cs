using System;
using FclEx.Utils;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency.Extensions
{
    public static class LightInjectExtensions
    {
        public static bool IsRegistered(this IServiceContainer services, Type type)
        {
            return services.CanGetInstance(type, string.Empty);
        }

        public static bool IsRegistered<TImpl>(this IServiceContainer services)
            where TImpl : class
        {
            return services.IsRegistered(typeof(TImpl));
        }

        public static ILifetime ToLightInjectLifetime(this ServiceLifetime lifeStyle)
        {
            switch (lifeStyle)
            {
                case ServiceLifetime.Singleton: return new PerContainerLifetime();
                case ServiceLifetime.Scoped: return new PerScopeLifetime();
                case ServiceLifetime.Transient: return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle), lifeStyle, null);
            }
        }

        public static IServiceContainer Add<T, TImpl>(this IServiceContainer services, ServiceLifetime lifeStyle)
            where T : class where TImpl : class, T
        {
            services.Register<T, TImpl>(lifeStyle.ToLightInjectLifetime());
            return services;
        }

        public static IServiceContainer TryAdd<T, TImpl>(this IServiceContainer services, ServiceLifetime lifeStyle)
            where T : class where TImpl : class, T
        {
            if (!services.IsRegistered(typeof(TImpl)))
                services.Add<T, TImpl>(lifeStyle);
            return services;
        }

        public static IServiceContainer Add<TImpl>(this IServiceContainer services, ServiceLifetime lifeStyle)
            where TImpl : class
        {
            return services.Add<TImpl, TImpl>(lifeStyle);
        }

        public static IServiceContainer TryAdd<TImpl>(this IServiceContainer services, ServiceLifetime lifeStyle)
            where TImpl : class
        {
            return services.TryAdd<TImpl, TImpl>(lifeStyle);
        }
        
        public static IServiceContainer Add<T, TImpl>(this IServiceContainer services, TImpl impl)
            where T : class where TImpl : class, T
        {
            services.RegisterInstance<T>(impl);
            return services;
        }

        public static IServiceContainer Add<TImpl>(this IServiceContainer services, TImpl impl)
            where TImpl : class
        {
            return services.Add<TImpl, TImpl>(impl);
        }

        public static IServiceContainer TryAdd<T, TImpl>(this IServiceContainer services, TImpl impl)
            where T : class where TImpl : class, T
        {
            if (!services.IsRegistered(typeof(TImpl)))
                services.Add<T, TImpl>(impl);
            return services;
        }

        public static IServiceContainer TryAdd<TImpl>(this IServiceContainer services, TImpl impl)
            where TImpl : class
        {
            return services.TryAdd<TImpl, TImpl>(impl);
        }

        public static IServiceContainer AddTransient<TImpl>(this IServiceContainer services)
            where TImpl : class
        {
            return Add<TImpl>(services, ServiceLifetime.Transient);
        }

        public static IServiceContainer TryAddTransient<TImpl>(this IServiceContainer services)
            where TImpl : class
        {
            return TryAdd<TImpl>(services, ServiceLifetime.Transient);
        }

        public static IServiceContainer AddTransient<T, TImpl>(this IServiceContainer services)
            where T : class where TImpl : class, T
        {
            return Add<T, TImpl>(services, ServiceLifetime.Transient);
        }

        public static IServiceContainer TryAddTransient<T, TImpl>(this IServiceContainer services)
            where T : class where TImpl : class, T
        {
            return TryAdd<T, TImpl>(services, ServiceLifetime.Transient);
        }
        

        public static IServiceContainer AddSingleton<TImpl>(this IServiceContainer services)
            where TImpl : class
        {
            return Add<TImpl>(services, ServiceLifetime.Singleton);
        }

        public static IServiceContainer TryAddSingleton<TImpl>(this IServiceContainer services)
            where TImpl : class
        {
            return TryAdd<TImpl>(services, ServiceLifetime.Singleton);
        }

        public static IServiceContainer AddSingleton<T, TImpl>(this IServiceContainer services)
            where T : class where TImpl : class, T
        {
            return Add<T, TImpl>(services, ServiceLifetime.Singleton);
        }

        public static IServiceContainer TryAddSingleton<T, TImpl>(this IServiceContainer services)
            where T : class where TImpl : class, T
        {
            return TryAdd<T, TImpl>(services, ServiceLifetime.Singleton);
        }

        public static IServiceContainer AddSingleton<T>(this IServiceContainer services, T impl)
            where T : class
        {
            return Add(services, impl);
        }

        public static IServiceContainer TryAddSingleton<TImpl>(this IServiceContainer services, TImpl impl)
            where TImpl : class
        {
            return TryAdd(services, impl);
        }

        public static IServiceContainer AddServices(this IServiceContainer services,
            Action<IServiceCollection> action)
        {
            Check.NotNull(action, nameof(action));
            var col = new ServiceCollection();
            action(col);
            if (!col.IsEmpty())
            {
                services.CreateServiceProvider(col);
            }
            return services;
        }
    }
}
