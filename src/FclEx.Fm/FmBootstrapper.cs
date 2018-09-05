using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using FclEx.Fm.Dependency;
using FclEx.Fm.Extensions;
using FclEx.Fm.Modules;
using FclEx.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fm
{
    /// <summary>
    /// This is the main class that is responsible to start entire ABP system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class FmBootstrapper : IDisposable
    {
        /// <summary>
        /// Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; }
        
        /// <summary>
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool _isDisposed;

        private FmModuleManager _moduleManager;
        private ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="FmBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="FmModule"/>.</param>
        /// <param name="optionsAction">An action to set options</param>
        private FmBootstrapper([NotNull] Type startupModule, [CanBeNull] Action<FmBootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            var options = new FmBootstrapperOptions();
            optionsAction?.Invoke(options);

            if (!typeof(FmModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(FmModule)}.");
            }

            StartupModule = startupModule;

            IocManager = options.IocManager;
            //PlugInSources = options.PlugInSources;

            _logger = NullLogger.Instance;

            //if (!options.DisableAllInterceptors)
            //{
            //    AddInterceptorRegistrars();
            //}
        }

        /// <summary>
        /// Creates a new <see cref="FmBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="FmModule"/>.</typeparam>
        /// <param name="optionsAction">An action to set options</param>
        public static FmBootstrapper Create<TStartupModule>([CanBeNull] Action<FmBootstrapperOptions> optionsAction = null)
            where TStartupModule : FmModule
        {
            return new FmBootstrapper(typeof(TStartupModule), optionsAction);
        }

        /// <summary>
        /// Creates a new <see cref="FmBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="FmModule"/>.</param>
        /// <param name="optionsAction">An action to set options</param>
        public static FmBootstrapper Create([NotNull] Type startupModule, [CanBeNull] Action<FmBootstrapperOptions> optionsAction = null)
        {
            return new FmBootstrapper(startupModule, optionsAction);
        }

        private void AddInterceptorRegistrars()
        {
            //ValidationInterceptorRegistrar.Initialize(IocManager);
            //AuditingInterceptorRegistrar.Initialize(IocManager);
            //EntityHistoryInterceptorRegistrar.Initialize(IocManager);
            //UnitOfWorkRegistrar.Initialize(IocManager);
            //AuthorizationInterceptorRegistrar.Initialize(IocManager);
        }

        /// <summary>
        /// Initializes the ABP system.
        /// </summary>
        public virtual void Initialize()
        {
            RegisterBootstrapper();
            IocManager.Build();

            ResolveLogger();
            try
            {
                //IocManager.IocContainer.Install(new AbpCoreInstaller());

                //IocManager.Resolve<AbpPlugInManager>().PlugInSources.AddRange(PlugInSources);
                //IocManager.Resolve<AbpStartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<FmModuleManager>();
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
                _logger = IocManager.Resolve<ILoggerFactory>().CreateLogger(typeof(FmBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            IocManager.Register(this);
        }

        /// <summary>
        /// Disposes the ABP system.
        /// </summary>
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
