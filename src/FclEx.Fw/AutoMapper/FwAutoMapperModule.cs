using System.Reflection;
using AutoMapper;
using FclEx.Fw.Dependency.Extensions;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using FclEx.Fw.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw.AutoMapper
{
    [DependsOn(typeof(FwKernelModule))]
    public class FwAutoMapperModule : FwModule
    {
        private readonly ITypeFinder _typeFinder;

        private static volatile bool _createdMappingsBefore;
        private static readonly object _syncObj = new object();

        public FwAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IFwAutoMapperConfiguration, FwAutoMapperConfiguration>();
            IocManager.Register<ObjectMapping.IObjectMapper, AutoMapperObjectMapper>();

            Configuration.Modules.FwAutoMapper().Configurators.Add(CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }


        private void Configurer(IMapperConfigurationExpression configuration)
        {
            FindAndAutoMapTypes(configuration);
            foreach (var configurator in Configuration.Modules.FwAutoMapper().Configurators)
            {
                configurator(configuration);
            }
        }

        private void CreateMappings()
        {
            lock (_syncObj)
            {

                if (Configuration.Modules.FwAutoMapper().UseStaticMapper)
                {
                    //We should prevent duplicate mapping in an application, since Mapper is static.
                    if (!_createdMappingsBefore)
                    {
                        Mapper.Initialize(Configurer);
                        _createdMappingsBefore = true;
                    }

                    IocManager.Container.AddSingleton<IConfigurationProvider>(Mapper.Configuration);
                    IocManager.Container.AddSingleton<IMapper>(Mapper.Instance);
                }
                else
                {
                    var config = new MapperConfiguration(Configurer);
                    IocManager.Container.AddSingleton<IConfigurationProvider>(config);
                    IocManager.Container.AddSingleton<IMapper>(config.CreateMapper());
                }
            }
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = _typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapToAttribute));
                }
            );

            Logger.LogDebug("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                Logger.LogDebug(type.Namespace + "." + type.ShortName());
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
        }
    }
}
