using FclEx.Fw.Dependency;
using FclEx.Fw.Extensions;

namespace FclEx.Fw.Configuration.Startup
{
    internal class FwStartupConfiguration : DictionaryBasedConfig, IFwStartupConfiguration
    {
        /// <summary>
        /// Reference to the IocManager.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public FwStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        public IModuleConfigurations Modules { get; private set; }

        public void Initialize()
        {
            Modules = IocManager.Resolve<IModuleConfigurations>();
        }

        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, () => IocManager.Resolve<T>());
        }
    }
}