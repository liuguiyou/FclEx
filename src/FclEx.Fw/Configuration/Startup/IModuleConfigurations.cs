namespace FclEx.Fw.Configuration.Startup
{
    /// <summary>
    /// Used to provide a way to configure modules.
    /// Create entension methods to this class to be used over <see cref="IFwStartupConfiguration.Modules"/> object.
    /// </summary>
    public interface IModuleConfigurations
    {
        IFwStartupConfiguration AbpConfiguration { get; }
    }
}