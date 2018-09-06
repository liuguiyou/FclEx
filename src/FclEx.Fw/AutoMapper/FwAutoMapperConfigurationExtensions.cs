using FclEx.Fw.Configuration.Startup;

namespace FclEx.Fw.AutoMapper
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure FclEx.Fw.AutoMapper module.
    /// </summary>
    public static class FwAutoMapperConfigurationExtensions
    {
        /// <summary>
        /// Used to configure FclEx.Fw.AutoMapper module.
        /// </summary>
        public static IFwAutoMapperConfiguration AbpAutoMapper(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IFwAutoMapperConfiguration>();
        }
    }
}