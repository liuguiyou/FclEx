using System;
using FclEx.Fw.Dependency;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw
{
    public class FwBootstrapperOptions
    {
        public IIocManager IocManager { get; set; } = Dependency.IocManager.Instance;

        public Action<ILoggingBuilder> LogConfigurer { get; set; }
    }
}
