using System;
using System.Threading.Tasks;
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
    }
}
