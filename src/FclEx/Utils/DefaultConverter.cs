using System;
using System.Linq.Expressions;

namespace FclEx.Utils
{
    public static class DefaultConverter<TOutput>
    {
        public static TOutput ConvertExpression<TInput>(TInput from) { return DefaultConverter<TInput, TOutput>.Instance.Invoke(from); }
        // public static TOutput ConvertDynamic(dynamic from) => from;
    }

    public static class DefaultConverter<TInput, TOutput>
    {
        public static Converter<TInput, TOutput> Instance { get; }

        static DefaultConverter()
        {
            var p = Expression.Parameter(typeof(TInput));
            Instance = Expression.Lambda<Converter<TInput, TOutput>>(Expression.Convert(p, typeof(TOutput)), p).Compile();
        }
    }
}