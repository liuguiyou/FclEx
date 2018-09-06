using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using AspectCore.Injector;
using FclEx.Fw.Configuration.Startup;
using FclEx.Fw.Dependency;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using FclEx.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fw
{
    public class FwBootstrapper : IDisposable
    {
        public Type StartupModule { get; }
        public IIocManager IocManager { get; }

        protected bool _isDisposed;

        private IFwModuleManager _moduleManager;
        private ILogger _logger = NullLogger.Instance;

        private FwBootstrapper([NotNull] Type startupModule, [CanBeNull] Action<FwBootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            var options = new FwBootstrapperOptions();
            optionsAction?.Invoke(options);

            if (!typeof(FwModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(FwModule)}.");
            }
            StartupModule = startupModule;
            IocManager = options.IocManager;

            IocManager.ServiceCollection
                .AddSingleton<IFwStartupConfiguration, FwStartupConfiguration>()
                .AddSingleton<IFwModuleManager, FwModuleManager>()
                .AddSingleton<IModuleConfigurations, ModuleConfigurations>()
                .AddLogging(options.LogConfigurer)
                .AddSingleton(NullLoggerProvider.Instance);
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
            RegisterBootstrapper();
            IocManager.Build();
            ResolveLogger();

            try
            {
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

        public virtual void Initialize()
        {
            var t = SimpleWatch.Do(InitializeInternal);
            _logger.LogDebug($"FwBootstrapper Initialize Finished. It takes {t.TotalSeconds:f2} seconds");
        }

        private void ResolveLogger()
        {
            _logger = IocManager.ServiceProvider.GetRequiredService<ILogger<FwBootstrapper>>();
        }

        private void RegisterBootstrapper()
        {
            IocManager.ServiceCollection.AddSingleton(this);
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
