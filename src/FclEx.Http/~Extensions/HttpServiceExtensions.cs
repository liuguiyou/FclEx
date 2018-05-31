using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Core;
using FclEx.Http.Services;

namespace FclEx.Http
{
    public static class HttpServiceExtensions
    {
        public static Task<HttpResponseItem> GetAsync(this IHttpService http, string url, string charSet = null, int? timeout = 10 * 1000, int retryTimes = 3)
        {
            var req = HttpRequestItem.CreateGetRequest(url);
            req.Compress();
            req.ResultChartSet = charSet;
            req.Timeout = timeout;
            return SendAsync(http, req);
        }

        public static async Task<HttpResponseItem> SendAsync(this IHttpService http, HttpRequestItem req, int retryTimes = 3)
        {
            if (retryTimes < 0) retryTimes = 0;
            for (var i = 0; i < retryTimes + 1; i++)
            {
                try
                {
                    var result = await http.ExecuteHttpRequestAsync(req, CancellationToken.None).ConfigureAwait(false);
                    return result;
                }
                catch (Exception ex)
                {
                    DebuggerHepler.WriteLine(ex.Message);
                }
            }
            return HttpResponseItem.ErrorResponse;
        }

        public static void AddCookies(this IHttpService http, IEnumerable<Cookie> cookies, string url = null)
        {
            foreach (var cookie in cookies)
            {
                http.AddCookie(cookie, url);
            }
        }

        public static void AddCookies(this IHttpService http, CookieCollection cc, string url = null) => AddCookies(http, cc.OfType<Cookie>(), url);
    }
}
