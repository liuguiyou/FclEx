using System;
using System.Collections.Generic;
using System.Linq;
using FclEx.Fm.Dependency;
using FclEx.Fm.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Fm.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class FmModuleManager : IFmModuleManager
    {
        public FmModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<FmModuleInfo> Modules => _modules;

        public ILogger Logger { get;  }

        private FmModuleCollection _modules;

        private readonly IIocManager _iocManager;
        //private readonly IAbpPlugInManager _abpPlugInManager;

        public FmModuleManager(IIocManager iocManager, ILogger logger)
        {
            _iocManager = iocManager;
            Logger = logger;
        }

        public virtual void Initialize(Type startupModule)
        {
            _modules = new FmModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.LogDebug("Shutting down has been started");

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.LogDebug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.LogDebug("Loading Abp modules...");

            var moduleTypes = FindAllModuleTypes(out var plugInModuleTypes).Distinct().ToList();

            Logger.LogDebug("Found " + moduleTypes.Count + " ABP modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.LogDebug("{0} modules loaded.", _modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = FmModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);
            
            //foreach (var plugInModuleType in _abpPlugInManager.PlugInSources.GetAllModules())
            //{
            //    if (modules.AddIfNotContains(plugInModuleType))
            //    {
            //        plugInModuleTypes.Add(plugInModuleType);
            //    }
            //}

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _iocManager.Resolve(moduleType) as FmModule;
                if (moduleObject == null)
                {
                    throw new Exception("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = _iocManager;
                //moduleObject.Configuration = _iocManager.Resolve<IAbpStartupConfiguration>();

                var moduleInfo = new FmModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.LogDebug("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                _iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in FmModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new Exception("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
