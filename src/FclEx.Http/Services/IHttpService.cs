using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.Core;
using FclEx.Http.Proxy;

namespace FclEx.Http.Services
{
    public interface IHttpService : IDisposable
    {
        /// <summary>
        /// 执行一个HTTP请求
        /// </summary>
        ValueTask<HttpRes> ExecuteHttpRequestAsync(HttpReq request, CancellationToken token = default);

        /// <summary>
        /// 获取一个cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        Cookie GetCookie(string name, string url);

        CookieCollection GetCookies(string url);
        
        void AddCookie(Cookie cookie, string url = null);

        List<Cookie> GetAllCookies();

        void ClearCookies(string url);

        void ClearAllCookies();

        IWebProxyExt WebProxy { get; set; }
    }
}
