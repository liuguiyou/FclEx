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
        ValueTask<HttpRes> ExecuteAsync(HttpReq request, CancellationToken token = default);

        Cookie GetCookie(Uri uri, string name);

        CookieCollection GetCookies(Uri uri);
        
        void AddCookie(Cookie cookie, Uri uri);

        List<Cookie> GetAllCookies();

        void ClearCookies(Uri uri);

        void ClearAllCookies();

        IWebProxyExt WebProxy { get; set; }
    }
}
