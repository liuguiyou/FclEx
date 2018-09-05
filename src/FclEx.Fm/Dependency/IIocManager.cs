using System;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fm.Dependency
{
    /// <summary>
    /// This interface is used to directly perform dependency injection tasks.
    /// </summary>
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        void Build(Func<IServiceCollection, IServiceProvider> buildFunc = null);
    }
}