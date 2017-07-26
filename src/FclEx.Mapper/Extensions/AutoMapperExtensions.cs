using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Extensions
{
    public static class AutoMapperExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source) => AutoMapper.Mapper.Map<TDestination>(source);

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) => AutoMapper.Mapper.Map(source, destination);
    }
}
