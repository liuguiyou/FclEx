using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Extensions
{
    public static class IocRegistrarExtensions
    {
        public static bool IsRegistered<T>(this IIocRegistrar resolver)
        {
            return resolver.IsRegistered(typeof(T));
        }

        public static IIocRegistrar Register<TType, TImpl>(this IIocRegistrar registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            return registrar.Register(typeof(TType), typeof(TImpl), lifeStyle);
        }

        public static IIocRegistrar Register(this IIocRegistrar registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(type, type, lifeStyle);
        }

        public static IIocRegistrar Register<T>(this IIocRegistrar registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where T : class
        {
            var t = typeof(T);
            return registrar.Register(t, t, lifeStyle);
        }

        public static IIocRegistrar Register(this IIocRegistrar registrar, Type type, object impl, 
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(type, r => impl, lifeStyle);
        }

        public static IIocRegistrar Register<T>(this IIocRegistrar registrar, 
            T impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.Register(typeof(T), impl, lifeStyle);
        }

        public static IIocRegistrar TryRegister(this IIocRegistrar registrar, Type type,
            Type impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            if (!registrar.IsRegistered(type))
                registrar.Register(type, impl, lifeStyle);
            return registrar;
        }

        public static IIocRegistrar TryRegister<TType, TImpl>(this IIocRegistrar registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            return registrar.TryRegister(typeof(TType), typeof(TImpl), lifeStyle);
        }

        public static IIocRegistrar RegisterIfNot<TType, TImpl>(this IIocRegistrar registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            return registrar.TryRegister(typeof(TType), typeof(TImpl), lifeStyle);
        }

        public static IIocRegistrar TryRegister(this IIocRegistrar registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.TryRegister(type, type, lifeStyle);
        }

        public static IIocRegistrar RegisterIfNot(this IIocRegistrar registrar, Type type,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            return registrar.TryRegister(type, type, lifeStyle);
        }

        public static IIocRegistrar TryRegister<T>(this IIocRegistrar registrar,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where T : class
        {
            var t = typeof(T);
            return registrar.TryRegister(t, t, lifeStyle);
        }

    }
}
