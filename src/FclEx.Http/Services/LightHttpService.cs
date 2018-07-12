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
using WebProxy = FclEx.Http.Proxy.WebProxy;

namespace FclEx.Http.Services
{
    public class LightHttpService : IHttpService
    {
        private WebProxy _webProxy = WebProxy.None;
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

        public LightHttpService(Uri uri, bool useCookie = true)
            : this(uri == null ? WebProxy.None : new WebProxy(uri), useCookie) { }

        public LightHttpService(string url, bool useCookie = true) 
            : this(url.IsNullOrEmpty() ? null : ObjectCache.CreateUri(url, true), useCookie) { }

        public LightHttpService(WebProxy proxy = null, bool useCookie = true)
        {
            if (useCookie)
                _cookieContainer = new CookieContainer();
            WebProxy = proxy;
        }

        public void Dispose()
        {
        }

        private static HttpWebRequest BuildRequest(HttpRequestItem request, WebProxy proxy, CookieContainer cc)
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

            if (proxy?.ProxyType == EnumProxyType.Http)
                req.Proxy = proxy;

            foreach (var header in request.HeaderMap.Where(h => !_notAddHeaderNames.Contains(h.Key)))
            {
                req.Headers.Add(header.Key, header.Value);
            }

            var cookies = request.HeaderMap.GetOrDefault(HttpConstants.Cookie)
                            ?? cc?.GetCookieHeader(request.RawUri);

            if (!cookies.IsNullOrEmpty())
                req.Headers.Add(HttpConstants.Cookie, cookies);

            return req;
        }

        private static void ReadCookies(HttpWebResponse response, CookieContainer cc)
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
                        DebuggerHepler.WriteLine("A cookie has been discarded. " + ex.Message);
                        DebuggerHepler.WriteLine(c.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DebuggerHepler.WriteLine("An error occurred while parsing cookie. " + ex.Message);
            }
        }

        private static void ReadHeader(HttpWebResponse response, HttpResponseItem responseItem)
        {
            foreach (var key in response.Headers.AllKeys)
            {
                var headers = response.Headers.GetValues(key);
                responseItem.Headers.AddRange(key, headers);
            }
        }

        private static async Task ReadContent(HttpWebResponse response, HttpResponseItem responseItem)
        {
            responseItem.ResponseChartSet = response.CharacterSet;

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

                switch (responseItem.RequestItem.ResultType)
                {
                    case HttpResultType.String:
                        var charset = responseItem.RequestItem.ResultChartSet.IsNullOrEmpty()
                            ? response.CharacterSet
                            : responseItem.RequestItem.ResultChartSet;

                        var contentEncoding = charset.IsNullOrEmpty()
                            ? Encoding.UTF8
                            : Encoding.GetEncoding(charset); // GetEncoding returns a cached instance with default settings. 

                        mem.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(mem, contentEncoding))
                        {
                            responseItem.ResponseString = sr.ReadToEnd();
                        }
                        break;

                    case HttpResultType.Byte:
                        responseItem.ResponseBytes = mem.ToArray();
                        break;
                }
            }
        }

        private static async ValueTask<HttpResponseItem> ExecuteAsync(HttpRequestItem requestItem, WebProxy proxy, CookieContainer cc, CancellationToken token)
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
                var responseItem = new HttpResponseItem { RequestItem = requestItem };
                var response = await req.GetHttpResponseAsync().DonotCapture();

                responses.Add(response);
                responseItem.RedirectUris.Add(response.ResponseUri);

                if (requestItem.ReadResultCookie)
                    ReadCookies(response, cc);

                while (response.IfRedirect())
                {
                    var uri = response.GetRedirectUri();
                    var tempReq = BuildRequest(HttpRequestItem.CreateGetRequest(uri), proxy, cc);
                    response = await tempReq.GetHttpResponseAsync().DonotCapture();
                    responses.Add(response);
                    responseItem.RedirectUris.Add(response.ResponseUri);

                    if (requestItem.ReadResultCookie)
                        ReadCookies(response, cc);
                }
                responseItem.StatusCode = response.StatusCode;

                if (requestItem.ReadResultHeader)
                    ReadHeader(response, responseItem);

                if (requestItem.ReadResultContent)
                    await ReadContent(response, responseItem).DonotCapture();

                // response.EnsureSuccessStatusCode();

                return responseItem;
            }
            finally
            {
                responses.ForEach(m => m?.Dispose());
                responses.Clear();
            }
        }

        public ValueTask<HttpResponseItem> ExecuteHttpRequestAsync(HttpRequestItem requestItem, CancellationToken token = default)
        {
            // 本方法依赖的类成员只有_webProxy和_cookieContainer，前者状态不可变，后者线程安全
            return ExecuteAsync(requestItem, _webProxy, _cookieContainer, token);
        }

        public Cookie GetCookie(string name, string url)
        {
            if (_cookieContainer == null) return null;
            var uri = ObjectCache.CreateUri(url, true);
            return _cookieContainer.GetCookies(uri)[name];
        }

        public CookieCollection GetCookies(string url)
        {
            if (_cookieContainer == null) return null;
            var uri = ObjectCache.CreateUri(url, true);
            return _cookieContainer.GetCookies(uri);
        }
        
        public void AddCookie(Cookie cookie, string url = null)
        {
            if (_cookieContainer == null) return;
            if (url == null) _cookieContainer.Add(cookie);
            else
            {
                var uri = ObjectCache.CreateUri(url, true);
                _cookieContainer.Add(uri, cookie);
            }
        }

        public List<Cookie> GetAllCookies()
        {
            return _cookieContainer.GetAllCookies();
        }

        public void ClearCookies(string url)
        {
            var cookies = GetCookies(url);
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

        public WebProxy WebProxy
        {
            get => _webProxy;
            set => _webProxy = value ?? _webProxy;
        }
    }
}
