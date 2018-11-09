using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FclEx.Http;
using FclEx.Http.Core;
using FclEx.Http.Services;
using FclEx.Utils;
using MoreLinq;

namespace FclEx.Benchmark
{
    public class HttpServiceRawTest
    {
        public static IList<string> Urls => new[]
        {
            "http://www.baidu.com/",
            "http://www.sina.com.cn/",
            "https://weibo.com/",
            "http://www.sohu.com/",
            "http://www.qq.com/",
        };

        public static IList<IHttpService> Services => new IHttpService[]
        {
            new HttpClientService(),
            new HttpClientExtService(),
            new LightHttpService()
        };

        public static async ValueTask RawTest(int rounds)
        {
            var reqs = Urls.Select(m => HttpReq.Get(m)
                .ReadResultCookie(false)
                .ReadResultContent(false)
                .ReadResultHeader(false)
                .AcceptBytes()).ToArray();

            foreach (var service in Services)
            {
                await RawTest(service, reqs, rounds).DonotCapture();
            }
        }

        public static async ValueTask RawTest(IHttpService service, IList<HttpReq> reqs, int rounds)
        {
            var before = GC.GetTotalMemory(true);
            var t = await SimpleWatch.DoAsync(async () =>
            {
                for (var i = 0; i < rounds; i++)
                {
                    foreach (var req in reqs)
                    {
                        var res = await service.ExecuteAsync(req).DonotCapture();
                        res.ThrowIfError();
                    }
                }
            }).DonotCapture();
            var after = GC.GetTotalMemory(true);
            Console.WriteLine($"[{service.GetType().SimpleName()}]: " +
                              $"Round: {rounds}, " +
                              $"Time: {t.TotalSeconds:f2}s, " +
                              $"Memory: {after - before}byte");
        }
    }
}
