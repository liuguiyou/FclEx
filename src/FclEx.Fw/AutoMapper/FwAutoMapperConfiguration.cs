using System;
using System.Collections.Generic;
using AutoMapper;

namespace FclEx.Fw.AutoMapper
{
    public class FwAutoMapperConfiguration : IFwAutoMapperConfiguration
    {
        public List<Action<IMapperConfigurationExpression>> Configurators { get; }

        public bool UseStaticMapper { get; set; }

        public FwAutoMapperConfiguration()
        {
            UseStaticMapper = true;
            Configurators = new List<Action<IMapperConfigurationExpression>>();
        }
    }
}