using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Http.Event;
using Xunit;

namespace FclEx.Http.Test
{
    public class ActionEventTests
    {
        public class Test
        {
            private static readonly DateTime _start = new DateTime(2000, 1, 1);
            public DateTime Start { get; set; } = _start;
            public string StartStr { get; set; } = _start.ToShort();
        }

        [Fact]
        public void Json_Test()
        {
            var action = ActionEvent.Ok(new Test());
            var json = action.ToJson();
            var obj = json.ToJToken().ToObject<ActionEvent<DateTime>>();
            Assert.True(obj.IsOk);
            Assert.Equal(action.Result.Start, action.Result.Start);
        }
    }
}
