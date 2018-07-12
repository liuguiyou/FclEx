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
        public static ValueTask<HttpResponseItem> GetAsync(this IHttpService http, string url, string charSet = null, int? timeout = 10 * 1000, int retryTimes = 3, int delaySeconds = 0)
        {
            var req = HttpRequestItem.CreateGetRequest(url)
                .Compress()
                .Timeout(timeout)
                .ChartSet(charSet);
            return SendAsync(http, req, retryTimes, delaySeconds);
        }

        public static ValueTask<HttpResponseItem> SendAsync(this IHttpService http, HttpRequestItem req, int retryTimes = 3, int delaySeconds = 0)
        {
            return ActionHelper.TryAsync(() => http.ExecuteHttpRequestAsync(req), retryTimes, delaySeconds, HttpResponseItem.CreateError);
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
