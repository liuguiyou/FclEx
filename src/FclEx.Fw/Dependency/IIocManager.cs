using System;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency
{
    /// <summary>
    /// This interface is used to directly perform dependency injection tasks.
    /// </summary>
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        IServiceCollection ServiceCollection { get; }
        IServiceProvider ServiceProvider { get; }
        void Build();
    }
}