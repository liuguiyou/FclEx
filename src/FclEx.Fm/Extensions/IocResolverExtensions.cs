using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FclEx.Fm.Dependency;

namespace FclEx.Fm.Extensions
{
    public static class IocResolverExtensions
    {
        public static T Resolve<T>(this IIocResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }

        public static IEnumerable<T> ResolveAll<T>(this IIocResolver resolver)
        {
            return resolver.ResolveAll(typeof(T)).Cast<T>();
        }
    }
}
