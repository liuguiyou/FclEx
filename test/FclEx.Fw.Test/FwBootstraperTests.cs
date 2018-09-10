using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.Dependency.Extensions;
using FclEx.Fw.Extensions;
using FclEx.Fw.Modules;
using Xunit;
using Xunit.Abstractions;

namespace FclEx.Fw.Test
{
    public class FwBootstraperTests : FwTests<FwBootstraperTests.MyTestModule>
    {
        public FwBootstraperTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Should_Call_Module_Events_Once()
        {
            var manager = IocManager.Resolve<IFwModuleManager>();
            manager.ShutdownModules();

            var testModule = IocManager.Resolve<MyTestModule>();
            var otherModule = IocManager.Resolve<MyOtherModule>();
            var anotherModule = IocManager.Resolve<MyAnotherModule>();



            testModule.PreInitializeCount.ShouldBe(1);
            testModule.InitializeCount.ShouldBe(1);
            testModule.PostInitializeCount.ShouldBe(1);
            testModule.ShutdownCount.ShouldBe(1);

            otherModule.PreInitializeCount.ShouldBe(1);
            otherModule.InitializeCount.ShouldBe(1);
            otherModule.PostInitializeCount.ShouldBe(1);
            otherModule.ShutdownCount.ShouldBe(1);
            otherModule.CallMeOnStartupCount.ShouldBe(1);

            anotherModule.PreInitializeCount.ShouldBe(1);
            anotherModule.InitializeCount.ShouldBe(1);
            anotherModule.PostInitializeCount.ShouldBe(1);
            anotherModule.ShutdownCount.ShouldBe(1);
        }

        [DependsOn(typeof(MyOtherModule))]
        [DependsOn(typeof(MyAnotherModule))]
        public class MyTestModule : MyEventCounterModuleBase
        {
            private readonly MyOtherModule _otherModule;

            public MyTestModule(MyOtherModule otherModule)
            {
                _otherModule = otherModule;
            }

            public override void PreInitialize()
            {
                base.PreInitialize();
                Assert.Equal(1, _otherModule.PreInitializeCount);
                _otherModule.CallMeOnStartup();
            }

            public override void Initialize()
            {
                base.Initialize();
                Assert.Equal(1, _otherModule.InitializeCount);
            }

            public override void PostInitialize()
            {
                base.PostInitialize();
                Assert.Equal(1, _otherModule.PostInitializeCount);
            }

            public override void Shutdown()
            {
                base.Shutdown();
                Assert.Equal(0, _otherModule.ShutdownCount);
                // Depended module should be shutdown after this module
            }
        }

        public class MyOtherModule : MyEventCounterModuleBase
        {
            public int CallMeOnStartupCount { get; private set; }

            public void CallMeOnStartup()
            {
                CallMeOnStartupCount++;
            }
        }

        public class MyAnotherModule : MyEventCounterModuleBase
        {

        }

        public abstract class MyEventCounterModuleBase : FwModule
        {
            public int PreInitializeCount { get; private set; }

            public int InitializeCount { get; private set; }

            public int PostInitializeCount { get; private set; }

            public int ShutdownCount { get; private set; }

            public override void PreInitialize()
            {
                Assert.NotNull(IocManager);
                Assert.NotNull(Configuration);
                PreInitializeCount++;
            }

            public override void Initialize()
            {
                InitializeCount++;
            }

            public override void PostInitialize()
            {
                PostInitializeCount++;
            }

            public override void Shutdown()
            {
                ShutdownCount++;
            }
        }
    }
}
