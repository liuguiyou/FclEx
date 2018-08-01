using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FclEx.Http.Core;
using FclEx.Utils;
using Xunit;
using Xunit.Abstractions;

namespace FclEx.Http.Test.HttpReqTests
{
    public class TimeoutTests
    {
        private readonly ITestOutputHelper _output;
        public TimeoutTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static string[] Urls { get; } =
        {
            "http://10.10.213.134/config/lookup",
            "http://10.10.213.134",
            "http://10.10.216.96/config/lookup",
            "http://10.10.216.96"
        };

        public static int[] Timeouts { get; } = { 3, 5, 7 };

        public static IEnumerable<object[]> CtorCases { get; } =
            Urls.SelectMany(m => Timeouts, (u, m) => new object[] { u, m });

        [Theory]
        [MemberData(nameof(CtorCases))]
        public async Task TestCtor(string url, int timeout)
        {
            var req = HttpReq.Get(url).Timeout(timeout * 1000);

            var time = await SimpleWatch.DoAsync(() => req.SendAsync());
            _output.WriteLine("耗时：" + time.TimeSpan.TotalSeconds + "秒");
            if (time.Ret.HasError)
            {
                _output.WriteLine("异常类型：" + time.Ret.Exception.GetInnermost().GetType());
                if (time.Ret.Exception is WebException e && e.Status == WebExceptionStatus.Timeout)
                    Assert.True(time.TimeSpan.TotalSeconds > timeout
                                && time.TimeSpan.TotalSeconds < timeout + 1);
            }
        }
    }
}
