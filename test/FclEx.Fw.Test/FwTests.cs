using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.Dependency;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using FclEx.Fw.Test.Logging;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FclEx.Fw.Test
{
    public abstract class FwTests<TModule> where TModule : FwModule
    {
        protected readonly ITestOutputHelper _output;

        protected FwTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            var boot = FwBootstrapper.Create<TModule>(o =>
            {
                o.IocManager.Container.TryAddSingleton(output);
                o.LogConfigurer = builder => { builder.AddProvider(new TestLoggerProvider(output)); };
            });
            IocManager = boot.IocManager;
            boot.Initialize();
        }

        public IIocManager IocManager { get; set; }
    }
}
