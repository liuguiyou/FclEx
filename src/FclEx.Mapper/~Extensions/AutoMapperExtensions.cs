using System.ComponentModel;
using System.Reflection;
using AutoMapper;

namespace FclEx
{
    public static class AutoMapperExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source) => AutoMapper.Mapper.Map<TDestination>(source);

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) => AutoMapper.Mapper.Map(source, destination);

        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map)
        {
            var sourceType = typeof(TSource);
            var properties = typeof(TDestination).GetTypeInfo().GetProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<AutoMapperIgnoreAttribute>(false);
                if (attribute == null || sourceType != attribute.SourceType) continue;
                map.ForMember(property.Name, opt => opt.Ignore());
            }
            return map;
        }
    }
}
