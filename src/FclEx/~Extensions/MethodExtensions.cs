﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FclEx
{
    public static class MethodExtensions
    {
        public static bool ArgumentListMatches(this MethodBase m, Type[] args)
        {
            // If there are less arguments, then it just doesn't matter.
            var pInfo = m.GetParameters();
            if (pInfo.Length < args.Length)
                return false;

            // Now, check compatibility of the first set of arguments.
            return !args.Where((arg, i) => ! pInfo[i].ParameterType.GetTypeInfo().IsAssignableFrom(arg)).Any() 
                && pInfo.Skip(args.Length).All(p => p.IsOptional);  // And make sure the last set of arguments are actually default!
        }

        public static bool IsAsyncMethod(this MethodInfo method)
        {
            // Obtain the custom attribute for the method.
            // The value returned contains the StateMachineType property.
            // Null is returned if the attribute isn't present for the method.
            var attrib = method.GetCustomAttribute<AsyncStateMachineAttribute>();
            return (attrib != null);
        }
    }
}
