using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Core;
using FclEx.Http.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Http.Services
{
    public class LightHttpService : IHttpService
    {
        private IWebProxyExt _webProxy = HttpProxy.None;
        private readonly CookieContainer _cookieContainer;
        private static readonly string[] _notAddHeaderNames =
        {
            HttpConstants.ContentType,
            HttpConstants.Cookie,
            HttpConstants.Referrer,
            HttpConstants.Host,
            HttpConstants.UserAgent,
            HttpConstants.ContentLength,
        };

        static LightHttpService()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public LightHttpService(Uri uri, ILogger logger = null, bool useCookie = true)
            : this(logger, uri == null ? HttpProxy.None : new HttpProxy(uri), useCookie) { }

        public LightHttpService(string url, ILogger logger = null, bool useCookie = true)
            : this(url.IsNullOrEmpty() ? null : new Uri(url), logger, useCookie) { }

        public LightHttpService(ILogger logger = null, IWebProxyExt proxy = null, bool useCookie = true)
        {
            if (useCookie)
                _cookieContainer = new CookieContainer();
            Logger = logger ?? NullLogger.Instance;
            WebProxy = proxy;
        }

        public void Dispose()
        {
            ClearAllCookies();
        }

        private static HttpWebRequest BuildRequest(HttpReq request, IWebProxyExt proxy, CookieContainer cc)
        {
            var req = (HttpWebRequest)WebRequest.Create(request.GetUrl());
            req.AllowAutoRedirect = false;
            req.ContentType = request.ContentType;
            req.Method = request.Method.ToString().ToUpper();
            req.Referer = request.Referrer;
            req.Host = request.Host;
            req.UserAgent = request.UserAgent;

            if (request.Timeout.HasValue)
                req.Timeout = request.Timeout.Value;

            if (proxy.ProxyType != ProxyType.None)
                req.Proxy = proxy;
            else
                req.Proxy = request.UseDefaultProxy ? WebRequest.DefaultWebProxy : null;


            foreach (var header in request.HeaderMap.Where(h => !_notAddHeaderNames.Contains(h.Key)))
            {
                req.Headers.Add(header.Key, header.Value);
            }

            var cookies = request.HeaderMap.GetOrDefault(HttpConstants.Cookie)
                            ?? cc?.GetCookieHeader(req.RequestUri);

            if (!cookies.IsNullOrEmpty())
                req.Headers.Add(HttpConstants.Cookie, cookies);

            return req;
        }

        private static void ReadCookies(HttpWebResponse response, CookieContainer cc, ILogger logger)
        {
            if (cc == null) return;

            var cookieStr = response.Headers[HttpConstants.SetCookie];
            if (cookieStr.IsNullOrEmpty()) return;

            var parser = new CookieParser(cookieStr);
            try
            {
                while (true)
                {
                    var c = parser.Get();
                    if (c == null) break;
                    try
                    {
                        var cookie = c.ToCookie();
                        if (cookie.Domain.IsNullOrEmpty())
                            cc.Add(response.ResponseUri, cookie);
                        else
                            cc.Add(cookie);
                    }
                    catch (Exception ex)
                    {
                        logger.LogTrace("A cookie has been discarded. " + ex.Message);
                        logger.LogTrace(c.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogTrace("An error occurred while parsing cookie. " + ex.Message);
            }
        }

        private static void ReadHeader(HttpWebResponse response, HttpRes res)
        {
            foreach (var key in response.Headers.AllKeys)
            {
                var headers = response.Headers.GetValues(key);
                res.Headers.AddRange(key, headers);
            }
        }

        private static async Task ReadContent(HttpWebResponse response, HttpRes res)
        {
            res.ResponseChartSet = response.CharacterSet;

            // 不能依赖content-length
            using (var mem = new MemoryStream())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (response.ContentEncoding?.ToLowerInvariant() == "gzip")
                    {
                        using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress))
                            await gZipStream.CopyToAsync(mem).DonotCapture();
                    }
                    else
                    {
                        await stream.CopyToAsync(mem).DonotCapture();
                    }
                }

                switch (res.Req.ResultType)
                {
                    case HttpResultType.String:
                        var charset = res.Req.ResultChartSet.IsNullOrEmpty()
                            ? response.CharacterSet
                            : res.Req.ResultChartSet;

                        var contentEncoding = charset.IsNullOrEmpty()
                            ? Encoding.UTF8
                            : Encoding.GetEncoding(charset); // GetEncoding returns a cached instance with default settings. 

                        mem.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(mem, contentEncoding))
                        {
                            res.ResponseString = sr.ReadToEnd();
                        }
                        break;

                    case HttpResultType.Byte:
                        res.ResponseBytes = mem.ToArray();
                        break;
                }
            }
        }

        private static async ValueTask<HttpRes> ExecuteAsync(HttpReq requestItem, IWebProxyExt proxy, CookieContainer cc, CancellationToken token, ILogger logger)
        {
            token.ThrowIfCancellationRequested();
            var responses = new List<HttpWebResponse>();

            try
            {
                var req = BuildRequest(requestItem, proxy, cc);
                var data = requestItem.GetBinaryData();
                if (!data.IsNullOrEmpty())
                {
                    req.ContentLength = data.Length;
                    //using (var stream = await req.GetRequestStreamAsync().ConfigureAwait(false))
                    //{
                    //    await stream.WriteAsync(data, 0, data.Length, token).DonotCapture();
                    //}

                    // use GetRequestStream() instead of GetRequestStreamAsync() to avoid hangs.
                    using (var stream = req.GetRequestStream())
                    {
                        await stream.WriteAsync(data, 0, data.Length, token).DonotCapture();
                    }
                }
                var responseItem = new HttpRes { Req = requestItem };
                var response = await req.GetHttpResponseAsync().DonotCapture();

                responses.Add(response);
                responseItem.RedirectUris.Add(response.ResponseUri);

                if (requestItem.ReadResultCookie)
                    ReadCookies(response, cc, logger);

                while (response.IfRedirect())
                {
                    var uri = response.GetRedirectUri();
                    var tempReq = BuildRequest(HttpReq.Get(uri), proxy, cc);
                    response = await tempReq.GetHttpResponseAsync().DonotCapture();
                    responses.Add(response);
                    responseItem.RedirectUris.Add(response.ResponseUri);

                    if (requestItem.ReadResultCookie)
                        ReadCookies(response, cc, logger);
                }
                responseItem.StatusCode = response.StatusCode;

                if (requestItem.ReadResultHeader)
                    ReadHeader(response, responseItem);

                if (requestItem.ReadResultContent)
                    await ReadContent(response, responseItem).DonotCapture();

                if (requestItem.ThrowOnNonSuccessCode)
                    response.EnsureSuccessStatusCode();

                return responseItem;
            }
            finally
            {
                responses.ForEach(m => m?.Dispose());
                responses.Clear();
            }
        }

        public ValueTask<HttpRes> ExecuteAsync(HttpReq req, CancellationToken token = default)
        {
            // 本方法依赖的类成员只有_webProxy和_cookieContainer，前者状态不可变，后者线程安全
            return ExecuteAsync(req, _webProxy, _cookieContainer, token, Logger);
        }

        public Cookie GetCookie(Uri uri, string name)
        {
            return _cookieContainer?.GetCookies(uri)[name];
        }

        public CookieCollection GetCookies(Uri uri)
        {
            return _cookieContainer?.GetCookies(uri);
        }

        public void AddCookie(Cookie cookie, Uri uri)
        {
            if (_cookieContainer == null) return;
            if (uri == null) _cookieContainer.Add(cookie);
            else
            {
                _cookieContainer.Add(uri, cookie);
            }
        }

        public IList<Cookie> GetAllCookies()
        {
            if (_cookieContainer == null) return Array.Empty<Cookie>();
            return _cookieContainer.GetAllCookies();
        }

        public void ClearCookies(Uri uri)
        {
            var cookies = GetCookies(uri);
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        public void ClearAllCookies()
        {
            if (_cookieContainer == null) return;
            var cookies = GetAllCookies();
            foreach (var cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        public IWebProxyExt WebProxy
        {
            get => _webProxy;
            set => _webProxy = value ?? _webProxy;
        }

        public ILogger Logger { get; }
    }
}
