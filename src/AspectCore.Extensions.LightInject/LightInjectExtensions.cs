using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;

namespace AspectCore.Extensions.LightInject
{
    public static class LightInjectExtensions
    {
        public static IServiceRegistry Add<TLifeTime>(this IServiceRegistry services,
            Type serviceType, Type implementingType)
            where TLifeTime : ILifetime, new()
        {
            services.Register(serviceType, implementingType, new TLifeTime());
            return services;
        }

        public static IServiceRegistry Add<TLifeTime>(this IServiceRegistry services, Type serviceType)
            where TLifeTime : ILifetime, new()
        {
            return services.Add<TLifeTime>(serviceType, serviceType);
        }

        public static IServiceRegistry Add<T, TImpl, TLifeTime>(this IServiceRegistry services)
            where T : class
            where TImpl : class, T
            where TLifeTime : ILifetime, new()
        {
            return services.Add<TLifeTime>(typeof(T), typeof(TImpl));
        }

        public static IServiceRegistry Add<TImpl, TLifeTime>(this IServiceRegistry services)
            where TImpl : class
            where TLifeTime : ILifetime, new()
        {
            return services.Add<TLifeTime>(typeof(TImpl));
        }

        public static IServiceRegistry AddSingleton<T>(this IServiceRegistry services, T instance)
        {
            services.RegisterInstance<T>(instance);
            return services;
        }

        public static IServiceRegistry AddSingleton(this IServiceRegistry services,
            Type serviceType, Type implementingType)
        {
            return Add<PerContainerLifetime>(services, serviceType, implementingType);
        }

        public static IServiceRegistry AddSingleton(this IServiceRegistry services, Type serviceType)
        {
            return Add<PerContainerLifetime>(services, serviceType);
        }

        public static IServiceRegistry AddSingleton<T, TImpl>(this IServiceRegistry services)
            where T : class
            where TImpl : class, T
        {
            return AddSingleton(services, typeof(T), typeof(TImpl));
        }

        public static IServiceRegistry AddSingleton<TImpl>(this IServiceRegistry services)
            where TImpl : class
        {
            return AddSingleton(services, typeof(TImpl));
        }

        public static IServiceRegistry AddTransient(this IServiceRegistry services,
            Type serviceType, Type implementingType)
        {
            return Add<PerRequestLifeTime>(services, serviceType, implementingType);
        }

        public static IServiceRegistry AddTransient(this IServiceRegistry services, Type serviceType)
        {
            return Add<PerRequestLifeTime>(services, serviceType);
        }

        public static IServiceRegistry AddTransient<T, TImpl>(this IServiceRegistry services)
            where T : class
            where TImpl : class, T
        {
            return AddTransient(services, typeof(T), typeof(TImpl));
        }

        public static IServiceRegistry AddTransient<TImpl>(this IServiceRegistry services)
            where TImpl : class
        {
            return AddTransient(services, typeof(TImpl));
        }
    }
}
