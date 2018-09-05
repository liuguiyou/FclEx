using System;
using System.Collections.Generic;

namespace FclEx.Fm.Dependency
{
    /// <summary>
    /// Define interface for classes those are used to resolve dependencies.
    /// </summary>
    public interface IIocResolver
    {
        object Resolve(Type type);

        IEnumerable<object> ResolveAll(Type type);
    }
}