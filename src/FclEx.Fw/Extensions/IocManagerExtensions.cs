using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FclEx.Fw.Dependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FclEx.Fw.Extensions
{
    public static class IocManagerExtensions
    {
        public static IIocManager Register(this IIocManager resolver, Type type, Type impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            resolver.ServiceCollection.Add(new ServiceDescriptor(type, impl, lifeStyle));
            return resolver;
        }

        public static IIocManager Register(this IIocManager resolver, Type type, Func<IServiceProvider, object> func, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            resolver.ServiceCollection.Add(new ServiceDescriptor(type, func, lifeStyle));
            return resolver;
        }

        public static bool IsRegistered(this IIocManager resolver, Type type)
        {
            return resolver.ServiceCollection.Any(x => x.ServiceType == type);
        }

        public static object Resolve(this IIocManager resolver, Type type)
        {
            return resolver.ServiceProvider.GetRequiredService(type);
        }

        public static T Resolve<T>(this IIocManager resolver)
        {
            return resolver.ServiceProvider.GetRequiredService<T>();
        }

        public static IEnumerable<T> ResolveAll<T>(this IIocManager resolver)
        {
            return resolver.ServiceProvider.GetServices<T>();
        }

        public static bool IsRegistered<T>(this IIocManager resolver)
        {
            return resolver.IsRegistered(typeof(T));
        }

        public static IIocManager Register<TType, TImpl>(this IIocManager registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            return registrar.Register(typeof(TType), typeof(TImpl), lifeStyle);
        }

        public static IIocManager Register(this IIocManager registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(type, type, lifeStyle);
        }

        public static IIocManager Register<T>(this IIocManager registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where T : class
        {
            var t = typeof(T);
            return registrar.Register(t, t, lifeStyle);
        }

        public static IIocManager Register(this IIocManager registrar, Type type, object impl,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(type, r => impl, lifeStyle);
        }

        public static IIocManager Register<T>(this IIocManager registrar,
            T impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(typeof(T), impl, lifeStyle);
        }

        public static IIocManager RegisterIfNot<TType, TImpl>(this IIocManager registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            registrar.ServiceCollection.TryAdd<TType, TImpl>(lifeStyle);
            return registrar;
        }

        public static IIocManager RegisterIfNot(this IIocManager registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            registrar.ServiceCollection.TryAdd(new ServiceDescriptor(type, type, lifeStyle));
            return registrar;
        }
    }
}
