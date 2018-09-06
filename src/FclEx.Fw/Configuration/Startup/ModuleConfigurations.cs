namespace FclEx.Fw.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IFwStartupConfiguration AbpConfiguration { get; private set; }

        public ModuleConfigurations(IFwStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;
        }
    }
}