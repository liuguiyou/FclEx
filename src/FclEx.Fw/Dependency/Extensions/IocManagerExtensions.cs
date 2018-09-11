using System;
using System.Collections.Generic;
using LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency.Extensions
{
    public static class IocManagerExtensions
    {
        public static IIocManager Register(this IIocManager resolver, Type type, Type impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            resolver.Container.Register(type, impl, lifeStyle.ToLightInjectLifetime());
            return resolver;
        }

        public static bool IsRegistered(this IIocManager resolver, Type type)
        {
            return resolver.Container.IsRegistered(type);
        }

        public static object Resolve(this IIocManager resolver, Type type)
        {
            return resolver.Container.GetInstance(type);
        }

        public static T Resolve<T>(this IIocManager resolver)
        {
            return resolver.Container.GetInstance<T>();
        }

        public static IEnumerable<T> ResolveAll<T>(this IIocManager resolver)
        {
            return resolver.Container.GetAllInstances<T>();
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

        public static IIocManager Register(this IIocManager registrar, Type type, object impl)
        {
            registrar.Container.RegisterInstance(type, impl);
            return registrar;
        }

        public static IIocManager Register<T>(this IIocManager registrar, T impl)
        {
            return registrar.Register(typeof(T), impl);
        }

        public static IIocManager RegisterIfNot<TType, TImpl>(this IIocManager registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            if (!registrar.IsRegistered<TType>())
                registrar.Container.Register<TType, TImpl>(lifeStyle.ToLightInjectLifetime());
            return registrar;
        }

        public static IIocManager RegisterIfNot(this IIocManager registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            if (!registrar.IsRegistered(type))
                registrar.Container.Register(type, lifeStyle.ToLightInjectLifetime());
            return registrar;
        }
    }
}
