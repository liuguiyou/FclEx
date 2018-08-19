﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Utils;
using Xunit;
using Xunit.Abstractions;

namespace FclEx.Test.Utils
{
    public class AsyncEventHandlerTests
    {
        private readonly ITestOutputHelper _helper;

        public AsyncEventHandlerTests(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        private class Tester
        {
            private readonly ITestOutputHelper _helper;

            public Tester(ITestOutputHelper helper)
            {
                _helper = helper;
                OnNotify += async (sender, tester) =>
                {
                    await TaskHelper.Delay(5);
                    _helper.WriteLine("default");
                };
            }

            public event AsyncEventHandler<Tester, Tester> OnNotify = (sender, args) => Task.CompletedTask;

            public Task Notify()
            {
                return OnNotify.InvokeAsync(this, this);
            }
        }

        [Fact]
        public async Task Test()
        {
            var tester = new Tester(_helper);
            tester.OnNotify += async (sender, e) =>
            {
                await TaskHelper.Delay(1);
                _helper.WriteLine("1 seconds");
            };

            tester.OnNotify += async (sender, e) =>
            {
                await TaskHelper.Delay(2);
                _helper.WriteLine("2 seconds");
            };

            await tester.Notify();
            _helper.WriteLine("Notify");

            await TaskHelper.Delay(10);
        }
    }
}
