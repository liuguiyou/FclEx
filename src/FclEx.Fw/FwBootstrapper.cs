using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using FclEx.Fw.Dependency;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using FclEx.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fw
{
    public class FwBootstrapper : IDisposable
    {
        public Type StartupModule { get; }
        public IIocManager IocManager { get; }

        protected bool _isDisposed;

        private FwModuleManager _moduleManager;
        private ILogger _logger;

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
            _logger = NullLogger.Instance;
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

        public virtual void Initialize()
        {
            RegisterBootstrapper();
            IocManager.Build();

            ResolveLogger();
            try
            {
                _moduleManager = IocManager.Resolve<FwModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.ToString());
                throw;
            }
        }

        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().CreateLogger(typeof(FwBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            IocManager.Register(this);
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
