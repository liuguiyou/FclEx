using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FclEx.Fm.Dependency.Registration
{
    public static class Classes
    {
        public static IHasTypes FromAssembly(Assembly assembly)
        {
            return new FromAssemblyDescriptor(assembly);
        }
    }
}
