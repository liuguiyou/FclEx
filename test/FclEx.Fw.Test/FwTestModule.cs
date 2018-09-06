using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.AutoMapper;
using FclEx.Fw.Modules;
using Microsoft.Extensions.Logging;

namespace FclEx.Fw.Test
{
    [DependsOn(typeof(FwAutoMapperModule))]
    public class FwTestModule : FwModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(GetType().Assembly);
        }
    }
}
