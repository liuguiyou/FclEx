using System;
using System.Reflection;
using FclEx.Fw.Domain.Entities;
using FclEx.Fw.Reflection;

namespace FclEx.Fw.Domain.Extensions
{
    public static class TypeFinderExtensions
    {
        public static Type[] GetEntityTypes(this ITypeFinder typeFinder)
        {
            var entityType = typeof(IEntity);
            var entityTypes = typeFinder.Find(m => m.IsInheritedFromGenericType(entityType) &&
                                                   !m.IsGenericType && !m.IsAbstract);
            return entityTypes;
        }
    }
}
