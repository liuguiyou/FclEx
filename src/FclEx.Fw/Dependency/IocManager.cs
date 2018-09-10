using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspectCore.Extensions.DependencyInjection;
using FclEx.Fw.Dependency.Registration.Conventional;
using FclEx.Fw.Extensions;
using FclEx.Utils;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency
{
    /// <summary>
    /// This class is used to directly perform dependency injection tasks.
    /// </summary>
    public class IocManager : IIocManager
    {
        private static readonly ContainerOptions _defaultOptions = new ContainerOptions
        {
            EnablePropertyInjection = false,
            DefaultServiceSelector = services => services.Last()
        };

        /// <summary>
        /// The Singleton instance.
        /// </summary>
        public static IocManager Instance { get; } = new IocManager();
        public IServiceContainer Container { get; } = new ServiceContainer(_defaultOptions);

        /// <summary>
        /// List of all registered conventional registrars.
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars
            = new List<IConventionalDependencyRegistrar>();

        public IocManager()
        {
            Container.RegisterInstance<IIocManager>(this);
            Container.RegisterInstance<IocManager>(this);
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