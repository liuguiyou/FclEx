using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency
{
    /// <summary>
    /// Define interface for classes those are used to register dependencies.
    /// </summary>
    public interface IIocRegistrar
    {
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);

        IIocRegistrar Register(Type type, Type impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton);

        IIocRegistrar Register(Type type, Func<IIocResolver, object> func, ServiceLifetime lifeStyle = ServiceLifetime.Singleton);

        bool IsRegistered(Type type);
    }
}