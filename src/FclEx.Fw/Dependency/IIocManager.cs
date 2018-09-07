using System;
using System.Reflection;
using LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency
{
    /// <summary>
    /// This interface is used to directly perform dependency injection tasks.
    /// </summary>
    public interface IIocManager : IDisposable
    {
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);
        void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config = null);
        IServiceContainer Container { get; }
    }
}