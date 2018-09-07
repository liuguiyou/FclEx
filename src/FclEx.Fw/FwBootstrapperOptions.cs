using System;
using FclEx.Fw.Dependency;
using FclEx.Fw.PlugIns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw
{
    public class FwBootstrapperOptions
    {
        private Action<ILoggingBuilder> _logConfigurer = builder => { };

        public IIocManager IocManager { get; set; } = Dependency.IocManager.Instance;

        public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public Action<ILoggingBuilder> LogConfigurer
        {
            get => _logConfigurer;
            set => _logConfigurer = value ?? _logConfigurer;
        }

        public PlugInSourceList PlugInSources { get; } = new PlugInSourceList();
    }
}
