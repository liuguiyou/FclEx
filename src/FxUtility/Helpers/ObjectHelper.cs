using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FxUtility.Extensions;

namespace FxUtility.Helpers
{
    public static class ObjectHelper
    {
        public static T CreateObject<T>(params object[] args)
        {
            return (T)typeof(T).CreateObject(args);
        }
    }
}
