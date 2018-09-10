using System;
using System.Collections.Generic;
using System.Text;
using EasyCaching.Core;

namespace FclEx.Fw.Caching.Extensions
{
    public static class EasyCachingProviderExtensions
    {
        public static bool TryGet<T>(this IEasyCachingProvider provider, string key, out T item)
        {
            var i = provider.Get<T>(key);
            item = i.Value;
            return i.HasValue;
        }
    }
}
