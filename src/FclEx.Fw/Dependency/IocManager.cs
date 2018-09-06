using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspectCore.Extensions.DependencyInjection;
using FclEx.Fw.Extensions;
using FclEx.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency
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
        private readonly Func<IServiceCollection, IServiceProvider> _buildFunc;

        /// <summary>
        /// Creates a new <see cref="IocManager"/> object.
        /// Normally, you don't directly instantiate an <see cref="IocManager"/>.
        /// This may be useful for test purposes.
        /// </summary>
        public IocManager() : this(new ServiceCollection(), null)
        {
        }

        public IocManager(IServiceCollection services, Func<IServiceCollection, IServiceProvider> buildFunc)
        {
            _buildFunc = buildFunc ?? (m => m.BuildAspectCoreServiceProvider()); ;
            Check.NotNull(services, nameof(services));
            ServiceCollection = services
                .AddSingleton<IIocManager>(this);
        }

        public void Build()
        {
            _serviceProvider = _buildFunc(ServiceCollection);
        }

        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar)
        {
            _conventionalRegistrars.Add(registrar);
        }

        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context = new ConventionalRegistrationContext(assembly, this, config);

            foreach (var registerer in _conventionalRegistrars)
            {
                registerer.RegisterAssembly(context);
            }
        }

        public void Dispose()
        {
        }
    }
}