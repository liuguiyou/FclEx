﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FclEx
{
    public static class ExpressionExtensions
    {
        private static Expression<T> Compose<T>(this Expression<T> left,
            Expression<T> right,
            Func<Expression, Expression, Expression> merge)
        {
            if (left == null) return right;
            var invExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<T>(merge(left.Body, invExpr), left.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            return left.Compose(right, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> OrIf<T>(this
            Expression<Func<T, bool>> left, bool condition,
            Expression<Func<T, bool>> right)
        {
            return condition ? Or(left, right) : left;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            return left.Compose(right, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> left,
            bool condition,
            Expression<Func<T, bool>> right)
        {
            return condition ? And(left, right) : left;
        }

        public static PropertyInfo GetProp<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (propertyLambda == null) throw new ArgumentNullException(nameof(propertyLambda));

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            var type = typeof(TSource);
            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }

        public static TSource SetPropIf<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            Func<TSource, TProperty, bool> condition, TProperty newValue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            var propertyInfo = source.GetProp(propertyLambda);
            var value = propertyInfo.GetValue(source).CastTo<TProperty>();
            if (condition(source, value))
                propertyInfo.SetValue(source, newValue);
            return source;
        }

        public static TSource SetPropIf<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            Func<TProperty, bool> condition, TProperty newValue)
        {
            return SetPropIf(source, propertyLambda, (s, p) => condition(p), newValue);
        }

        public static TSource SetProp<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            TProperty newValue)
        {
            var propertyInfo = source.GetProp(propertyLambda);
            propertyInfo.SetValue(source, newValue);
            return source;
        }

        public static TSource SetPropIfNull<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            TProperty newValue)
        {
            return SetPropIf(source, propertyLambda, (s, p) => p == null, newValue);
        }

        public static TSource SetPropIfDefault<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            TProperty newValue)
        {
            return SetPropIf(source, propertyLambda, (s, p) => p.IsDefault(), newValue);
        }

        public static TSource UpdateProp<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            TProperty newValue,
            Func<TProperty, bool> newValueCondition = null)
        {
            if (newValueCondition == null || newValueCondition(newValue))
            {
                var propertyInfo = source.GetProp(propertyLambda);
                propertyInfo.SetValue(source, newValue);
            }
            return source;
        }

        public static TSource UpdatePropIfNotEmpty<TSource>(this TSource source,
            Expression<Func<TSource, string>> propertyLambda,
            string newValue)
        {
            return UpdateProp(source, propertyLambda, newValue, n => !n.IsNullOrEmpty());
        }
    }
}
