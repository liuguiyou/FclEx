using System;
using System.Collections.Generic;
using System.Linq;
using FclEx.Fw.Modules;

namespace FclEx.Fw.PlugIns
{
    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(FwModule.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}