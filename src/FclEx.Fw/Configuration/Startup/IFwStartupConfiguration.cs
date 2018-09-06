using System;
using FclEx.Fw.Dependency;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw.Configuration.Startup
{
    public interface IFwStartupConfiguration : IDictionaryBasedConfig
    {
        IIocManager IocManager { get; }

        IModuleConfigurations Modules { get; }

        T Get<T>();

        void Initialize();
    }
}
