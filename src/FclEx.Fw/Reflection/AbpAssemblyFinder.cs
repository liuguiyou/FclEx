using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FclEx.Fw.Modules;

namespace FclEx.Fw.Reflection
{
    public class AbpAssemblyFinder : IAssemblyFinder
    {
        private readonly IFwModuleManager _moduleManager;

        public AbpAssemblyFinder(IFwModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}