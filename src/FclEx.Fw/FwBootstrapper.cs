using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Injector;
using EasyCaching.InMemory;
using FclEx.Fw.Configuration.Startup;
using FclEx.Fw.Dependency;
using FclEx.Fw.Dependency.Extensions;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using FclEx.Fw.PlugIns;
using FclEx.Fw.Reflection;
using FclEx.Utils;
using JetBrains.Annotations;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fw
{
    public class FwBootstrapper : IDisposable
    {
        private readonly FwBootstrapperOptions _options = new FwBootstrapperOptions();

        public Type StartupModule { get; }
        public PlugInSourceList PlugInSources => _options.PlugInSources;
        public IIocManager IocManager => _options.IocManager;

        protected bool _isDisposed;

        private IFwModuleManager _moduleManager;
        private ILogger _logger = NullLogger.Instance;

        private FwBootstrapper([NotNull] Type startupModule, [CanBeNull] Action<FwBootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            optionsAction?.Invoke(_options);

            if (!typeof(FwModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(FwModule)}.");
            }
            StartupModule = startupModule;
        }

        public static FwBootstrapper Create<TStartupModule>([CanBeNull] Action<FwBootstrapperOptions> optionsAction = null)
            where TStartupModule : FwModule
        {
            return new FwBootstrapper(typeof(TStartupModule), optionsAction);
        }

        public static FwBootstrapper Create([NotNull] Type startupModule, [CanBeNull] Action<FwBootstrapperOptions> optionsAction = null)
        {
            return new FwBootstrapper(startupModule, optionsAction);
        }

        protected virtual void InitializeInternal()
        {
            RegisterServices();
            RegisterBootstrapper();
            RegisterModules();
            IocManager.Container.CreateServiceProvider(_options.ServiceCollection);
            IocManager.Container.Compile();
            ResolveLogger();

            try
            {
                IocManager.Resolve<IFwPlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<IFwStartupConfiguration>().Initialize();
                _moduleManager = IocManager.Resolve<IFwModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.ToString());
                throw;
            }
        }

        private void RegisterServices()
        {
            _options.ServiceCollection
                .AddDefaultInMemoryCache()
                .AddSingleton<IAssemblyFinder, AbpAssemblyFinder>()
                .AddSingleton<ITypeFinder, TypeFinder>()
                .AddSingleton<IFwPlugInManager, FwPlugInManager>()
                .AddSingleton<IFwStartupConfiguration, FwStartupConfiguration>()
                .AddSingleton<IFwModuleManager, FwModuleManager>()
                .AddSingleton<IModuleConfigurations, ModuleConfigurations>()
                .AddLogging(_options.LogConfigurer)
                .AddSingleton(NullLoggerProvider.Instance);
        }

        private void RegisterModules()
        {
            var modules = FwModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(StartupModule);
            foreach (var moduleType in modules)
            {
                IocManager.RegisterIfNot(moduleType);
            }
        }

        public virtual void Initialize()
        {
            var t = SimpleWatch.Do(InitializeInternal);
            _logger.LogDebug($"FwBootstrapper Initialize Finished. It takes {t.TotalSeconds:f2} seconds");
        }

        private void ResolveLogger()
        {
            _logger = IocManager.Resolve<ILogger<FwBootstrapper>>();
        }

        private void RegisterBootstrapper()
        {
            IocManager.Container.AddSingleton(this);
        }

        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _moduleManager?.ShutdownModules();
        }
    }
}
