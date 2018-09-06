using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.Dependency;

namespace FclEx.Fw.ObjectMapping
{
    public sealed class NullObjectMapper : IObjectMapper, ISingletonDependency
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullObjectMapper Instance { get; } = new NullObjectMapper();

        public TDestination Map<TDestination>(object source)
        {
            throw new Exception("Abp.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            throw new Exception("Abp.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }
    }
}
