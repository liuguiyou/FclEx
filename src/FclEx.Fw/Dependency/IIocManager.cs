using System;
using System.Reflection;
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
        IServiceCollection ServiceCollection { get; }
        IServiceProvider ServiceProvider { get; }
        void Build();
    }
}