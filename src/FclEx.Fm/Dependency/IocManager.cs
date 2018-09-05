using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FclEx.Fm.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fm.Dependency
{
    /// <summary>
    /// This class is used to directly perform dependency injection tasks.
    /// </summary>
    public class IocManager : IIocManager
    {
        /// <summary>
        /// The Singleton instance.
        /// </summary>
        public static IocManager Instance { get; private set; } = new IocManager();

        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                    throw new Exception("Must build before use methods of IIocResolver");
                return _serviceProvider;
            }

            private set => _serviceProvider = value;
        }

        public IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// List of all registered conventional registrars.
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars
            = new List<IConventionalDependencyRegistrar>();

        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new <see cref="IocManager"/> object.
        /// Normally, you don't directly instantiate an <see cref="IocManager"/>.
        /// This may be useful for test purposes.
        /// </summary>
        public IocManager()
        {
            ServiceCollection = new ServiceCollection()
                .AddSingleton<IIocRegistrar>(this)
                .AddSingleton<IIocResolver>(this)
                .AddSingleton<IIocManager>(this);
        }

        public void Build(Func<IServiceCollection, IServiceProvider> buildFunc = null)
        {
            buildFunc = buildFunc ?? (m => m.BuildServiceProvider());
            ServiceProvider = buildFunc(ServiceCollection);
        }

        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar)
        {
            _conventionalRegistrars.Add(registrar);
        }

        public IIocRegistrar Register(Type type, Type impl, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            ServiceCollection.Add(new ServiceDescriptor(type, impl, lifeStyle));
            return this;
        }

        public IIocRegistrar Register(Type type, Func<IIocResolver, object> func, ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
        {
            ServiceCollection.Add(new ServiceDescriptor(type, p => func(this), lifeStyle));
            return this;
        }

        public bool IsRegistered(Type type)
        {
            return ServiceCollection.Any(x => x.ServiceType == type);
        }

        public object Resolve(Type type)
        {
            return ServiceProvider.GetRequiredService(type);
        }

        public object TryResolve(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return ServiceProvider.GetServices(type);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}