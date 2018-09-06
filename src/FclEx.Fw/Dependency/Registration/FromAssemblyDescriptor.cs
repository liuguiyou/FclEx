using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FclEx.Fw.Dependency.Registration
{
    /// <summary>
	///   Selects a set of types from an assembly.
	/// </summary>
	public class FromAssemblyDescriptor : IHasTypes
    {
        private readonly Assembly _assembly;
        private readonly IEnumerable<Type> _types;

        public FromAssemblyDescriptor(Assembly assembly)
        {
            _assembly = assembly;
            _types = assembly.GetTypes()
                .Where(m => !m.IsAbstract && m.IsClass);
        }

        public IEnumerable<Type> GetTypes() => _types;
    }
}