using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FclEx
{
    public static class ExpressionExtensions
    {
        private static Expression<T> Compose<T>(this Expression<T> left, Expression<T> right, Func<Expression, Expression, Expression> merge)
        {
            if (left == null) return right;
            var invExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<T>(merge(left.Body, invExpr), left.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return left.Compose(right, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return left.Compose(right, Expression.AndAlso);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
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

        public static TSource SetPropertyIf<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda, Func<TSource, TProperty, bool> condition, TProperty newValue)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            var propertyInfo = source.GetPropertyInfo(propertyLambda);
            var value = propertyInfo.GetValue(source).CastTo<TProperty>();
            if (condition(source, value))
                propertyInfo.SetValue(source, newValue);
            return source;
        }

        public static TSource SetPropertyIf<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda, Func<TProperty, bool> condition, TProperty newValue)
        {
            return SetPropertyIf(source, propertyLambda, (s, p) => condition(p), newValue);
        }

        public static TSource SetProperty<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda, TProperty newValue)
        {
            var propertyInfo = source.GetPropertyInfo(propertyLambda);
            propertyInfo.SetValue(source, newValue);
            return source;
        }

        public static TSource SetPropertyIfNull<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda, TProperty newValue)
        {
            return SetPropertyIf(source, propertyLambda, (s, p) => p == null, newValue);
        }

        public static TSource SetPropertyIfDefault<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda, TProperty newValue)
        {
            return SetPropertyIf(source, propertyLambda, (s, p) => p.IsDefault(), newValue);
        }
    }
}
