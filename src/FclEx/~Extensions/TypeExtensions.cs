﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FclEx.Utils;

namespace FclEx
{
    public static class TypeExtensions
    {
        public static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static bool SequenceAssignableFrom(this IEnumerable<Type> first, IEnumerable<Type> second)
        {
            var comparer = EqualityComparer<Type>.Default;
            using (var e1 = first.GetEnumerator())
            {
                using (var e2 = second.GetEnumerator())
                {
                    while (e1.MoveNext())
                    {
                        if (!e2.MoveNext()) return false;
                        else if (!(comparer.Equals(e1.Current, e2.Current) || e1.Current.GetTypeInfo().IsAssignableFrom(e2.Current)))
                            return false;
                    }
                    if (e2.MoveNext())
                        return false;
                }
            }
            return true;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public static object GetDefaultValueV2(this Type type)
        {
            // Validate parameters.
            if (type == null) throw new ArgumentNullException(nameof(type));

            // We want an Func<object> which returns the default.
            // Create that expression here.
            var e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

            // Compile and return the value.
            return e.Compile()();
        }

        public static object CreateObject(this Type type, params object[] args)
        {
            Check.NotNull(type, nameof(type));

            if (args.IsNullOrEmpty()) return Activator.CreateInstance(type);

            var argsType = args.Select(a => a.GetType()).ToArray();
            var ctor = type.GetConstructors().FirstOrDefault(m => m.ArgumentListMatches(argsType));
            if (ctor != null)
            {
                var paras = ctor.GetParameters();
                if (paras.Length > args.Length)
                {
                    args = args.Concat(paras.Skip(args.Length).Select(m => m.RawDefaultValue)).ToArray();
                }
                return ctor.Invoke(args);
            }

            throw new MissingMethodException();
        }

        public static bool IsInheritedFromGenericType(this Type type, Type genericType)
        {
            return GetGenericInterface(type, genericType) != null;
        }

        public static Type GetGenericInterface(this Type type, Type genericType)
        {
            return type.GetTypeInfo().GetInterfaces().FirstOrDefault(x =>
                x.GetTypeInfo().IsGenericType &&
                x.GetGenericTypeDefinition() == genericType);
        }

        public static Type GetAnyElementType(this Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GenericTypeArguments[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetGenericInterface(typeof(IEnumerable<>));
            if (enumType != null)
                return enumType.GenericTypeArguments[0];

            return null;
        }
    }
}
