using System;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Event;
using Xunit;
using Xunit.Abstractions;

namespace FclEx.Http.Test
{
    public class ActionEventTaskTests
    {
        private readonly ITestOutputHelper _output;

        public ActionEventTaskTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static Task<ActionEvent> Foo()
        {
            return ActionEvent.EmptyOkEvent.ToTask();
        }

        private static async Task<ActionEvent> Bar()
        {
            await TaskHelper.Delay(1);
            return ActionEvent.EmptyOkEvent;
        }

        private static Task ThrowAsync() => throw new Exception();

        private static void Throw() => throw new Exception();

        [Fact]
        public async Task TestOk()
        {
            await Assert.ThrowsAsync<Exception>(async () =>
                await Foo()
                    .Ok(o => Throw())
                    .Error(e => { }));

            await Assert.ThrowsAsync<Exception>(async () =>
                await Foo()
                    .Ok(o => ThrowAsync())
                    .Error(e => { }));

            await Foo()
                .Ok(o => { })
                .Error(e => Throw());

            await Foo()
                .Ok(o => { })
                .Error(e => ThrowAsync());

            await Foo()
                .Error(e => Throw())
                .Ok(o => { });

            await Foo()
                .Error(e => ThrowAsync())
                .Ok(o => { });

            await Assert.ThrowsAsync<Exception>(async () =>
                await Foo()
                    .Error(e => { })
                    .Ok(o => Throw()));

            await Assert.ThrowsAsync<Exception>(async () =>
                await Foo()
                    .Error(e => { })
                    .Ok(o => ThrowAsync()));

        }

        [Fact]
        public async Task TestDelay()
        {
            var startTime = DateTime.UtcNow;
            var task = Bar().Ok(o => { });
            var endTime = DateTime.UtcNow;
            Assert.True((endTime - startTime).TotalSeconds < 0.1);
            await task;
            var endTimeFinal = DateTime.UtcNow;
            Assert.True((endTimeFinal - startTime).TotalSeconds > 0.9);
        }
    }
}
