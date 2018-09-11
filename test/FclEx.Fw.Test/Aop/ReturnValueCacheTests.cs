using System;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Fw.Aop;
using FclEx.Fw.Dependency;
using FclEx.Fw.Dependency.Extensions;
using FclEx.Helpers;
using FclEx.Utils;
using Xunit;
using Xunit.Abstractions;

namespace FclEx.Fw.Test.Aop
{
    public class ReturnValueCacheTests : FwTests<FwTestModule>
    {
        public const int CacheMaxMilliseconds = 100;
        public const int SleepMilliseconds = 200;

        public class Model
        {
            public string Id { get; }
            public Model(string id) { Id = id; }
        }

        public interface IService
        {
            int Id { get; }

            [ReturnValueCache(IsStatic = true)]
            Model GetStatic(int id);

            [ReturnValueCache]
            Model Get(int id);
        }

        public class Service : IService
        {
            private static int _id = short.MinValue;
            public int Id { get; }

            public Service()
            {
                Id = Interlocked.Increment(ref _id);
            }

            public Model GetStatic(int id)
            {
                Thread.Sleep(SleepMilliseconds);
                return new Model(id.ToString());
            }

            public Model Get(int id)
            {
                Thread.Sleep(SleepMilliseconds);
                return new Model($"{_id}_{id}");
            }
        }

        public ReturnValueCacheTests(ITestOutputHelper output) : base(output)
        {
            IocManager.Container.TryAddTransient<IService, Service>();
        }

        [Theory]
        [MemberData(nameof(Numbers))]
        public void TestSingel(int no)
        {
            var service = IocManager.Resolve<IService>();
            var itemFromStatic = service.GetStatic(no);
            var itemFromInstace = service.Get(no);

            for (var i = 0; i < 3; i++)
            {
                var (tempItem, t) = SimpleWatch.Do(() => service.Get(no));
                Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds);
                Assert.Equal(itemFromInstace.Id, tempItem.Id);
            }
            for (var i = 0; i < 3; i++)
            {
                var (tempItem, t) = SimpleWatch.Do(() => service.GetStatic(no));
                Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds);
                Assert.Equal(itemFromStatic.Id, tempItem.Id);
            }
        }

        [Theory]
        [MemberData(nameof(Numbers))]
        public void TestMultiple(int no)
        {
            var service = IocManager.Resolve<IService>();
            var itemFromStatic = service.GetStatic(no);
            for (var i = 0; i < 3; i++)
            {
                var tempService = IocManager.Resolve<IService>();
                var (tempitemFromStatic, timeFromStatic) = SimpleWatch.Do(() => tempService.GetStatic(no));
                var (tempItemFromInstace, timeFromInstace) = SimpleWatch.Do(() => tempService.Get(no));
                Assert.True(timeFromStatic.TotalMilliseconds < CacheMaxMilliseconds);
                Assert.True(Math.Abs(timeFromInstace.TotalMilliseconds - SleepMilliseconds) < 50);
                Assert.Equal(itemFromStatic.Id, tempitemFromStatic.Id);
                Assert.Equal($"{tempService.Id}_{no}", tempItemFromInstace.Id);
            }
        }
    }
}
