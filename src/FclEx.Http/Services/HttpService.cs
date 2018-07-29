﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Core;
using FclEx.Http.Proxy;
using FclEx.Http.SocksUtil.SocksPort;
using HttpProxy = FclEx.Http.Proxy.HttpProxy;

namespace FclEx.Http.Services
{
    [Obsolete]
    public sealed class HttpService : IHttpService
    {
        public static IHttpService GlobalService { get; } = new HttpService();
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private HttpClient _httpClient;
        private IWebProxyExt _webProxy = HttpProxy.None;

        private static readonly string[] _notAddHeaderNames = { HttpConstants.ContentType, HttpConstants.Cookie, HttpConstants.UserAgent };
        private static readonly Regex _rexCharset = new Regex(@"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public HttpService() : this((HttpProxy)null)
        {
        }

        public HttpService(IWebProxyExt proxy)
        {
            WebProxy = proxy;
            _httpClient = CreateHttpClient(_cookieContainer, proxy);
        }

        public HttpService(Uri uri) : this(new HttpProxy(uri)) { }

        public HttpService(string url) : this(new Uri(url)) { }

        public HttpService(HttpClientHandler handler)
        {
            handler = handler ?? throw new ArgumentNullException(nameof(handler));
            handler.UseCookies = false;
            _httpClient = new HttpClient(handler);
        }

        private static HttpClient CreateHttpClient(CookieContainer cc, IWebProxyExt proxy = null)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                CookieContainer = cc,
                UseCookies = false,
                MaxConnectionsPerServer = 64,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            if (proxy != null)
            {
                handler.UseProxy = true;
                handler.Proxy = proxy;
            }
            else
            {
                handler.UseProxy = false;
                handler.Proxy = null;
            }
            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
            httpClient.DefaultRequestHeaders.Add(HttpConstants.UserAgent, HttpConstants.DefaultUserAgent);
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }

        private static HttpClient CreateSockClient(string socksAddress, int socksPort)
        {
            var handler = new SocksPortHandler(socksAddress, socksPort);
            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
            httpClient.DefaultRequestHeaders.Add(HttpConstants.UserAgent, HttpConstants.DefaultUserAgent);
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }

        private HttpRequestMessage GetHttpRequest(HttpReq item)
        {
            var request = new HttpRequestMessage(new HttpMethod(item.Method.ToString().ToUpper()), item.GetUrl())
            {
                Content = new ByteArrayContent(item.GetBinaryData()) { Headers = { ContentType = MediaTypeHeaderValue.Parse(item.ContentType) } }
            };

            foreach (var header in item.HeaderMap.Where(h => !_notAddHeaderNames.Contains(h.Key)))
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var cookies = item.HeaderMap.GetOrDefault(HttpConstants.Cookie) ??
                          _cookieContainer.GetCookieHeader(request.RequestUri);
            if (!cookies.IsNullOrEmpty())
            {
                request.Headers.Add(HttpConstants.Cookie, cookies);
            }

            return request;
        }

        private static void ReadHeader(HttpResponseMessage response, HttpRes res)
        {
            foreach (var header in response.Headers)
            {
                res.Headers.AddRange(header.Key, header.Value);
            }
        }

        private static async Task ReadContentAsync(HttpResponseMessage response, HttpRes res)
        {
            switch (res.Req.ResultType)
            {
                case HttpResultType.String:
                    res.ResponseString = await response.Content.ReadAsStringAsync().DonotCapture();
                    break;

                case HttpResultType.Byte:
                    res.ResponseBytes = await response.Content.ReadAsByteArrayAsync().DonotCapture();
                    break;
            }
            foreach (var header in response.Content.Headers)
            {
                res.Headers.AddRange(header.Key, header.Value);
            }
        }

        private void SetProxy(IWebProxyExt proxy)
        {
            if (Equals(_webProxy, proxy)) return;

            switch (proxy.ProxyType)
            {
                case ProxyType.None:
                case ProxyType.Http:
                    _httpClient = CreateHttpClient(_cookieContainer, proxy);
                    break;

                case ProxyType.Socks:
                    _httpClient = CreateSockClient(proxy.Host, proxy.Port);
                    break;

                case ProxyType.Https:
                default:
                    throw new ArgumentOutOfRangeException(nameof(proxy.ProxyType), proxy.ProxyType, null);
            }
            _webProxy = proxy;
        }

        private void SetHttpProxy(IWebProxyExt proxy)
        {
            var client = CreateHttpClient(_cookieContainer, proxy);
            var oldClient = _httpClient;
            _httpClient = client;
            oldClient.Dispose();
        }

        private static void ReadCookies(HttpResponseMessage response, CookieContainer cc)
        {
            if (!response.Headers.TryGetValues(HttpConstants.SetCookie, out var cookies)) return;

            foreach (var cookieStr in cookies)
            {
                try
                {
                    var cookie = new CookieParser(cookieStr).Get()?.ToCookie();
                    if (cookie == null) continue;

                    if (cookie.Domain.IsNullOrEmpty())
                        cc.Add(response.RequestMessage.RequestUri, cookie);
                    else
                        cc.Add(cookie);
                }
                catch (Exception ex)
                {
                    DebuggerHepler.WriteLine($"A cookie has been discarded. [{cookieStr}][{ex.Message}]");
                }
            }
        }

        public async ValueTask<HttpRes> ExecuteAsync(HttpReq requestItem, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var responses = new List<HttpResponseMessage>();
            try
            {
                if (requestItem.Timeout.HasValue && token.IsDefault())
                {
                    token = new CancellationTokenSource(requestItem.Timeout.Value).Token;
                }
                var responseItem = new HttpRes { Req = requestItem };
                var httpRequest = GetHttpRequest(requestItem);
                var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, token).DonotCapture();
                responses.Add(response);
                responseItem.RedirectUris.Add(response.RequestMessage.RequestUri);

                if (requestItem.ReadResultCookie)
                    ReadCookies(response, _cookieContainer);

                while ((response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Moved)
                    && response.Headers.Location != null)
                {
                    var uri = response.Headers.Location;
                    if (!uri.IsAbsoluteUri)
                    {
                        var originUri = response.RequestMessage.RequestUri;
                        uri = new Uri(originUri, uri);
                    }
                    var req = new HttpRequestMessage(HttpMethod.Get, uri);
                    var cookies = _cookieContainer.GetCookieHeader(uri);
                    if (!cookies.IsNullOrEmpty()) req.Headers.Add(HttpConstants.Cookie, cookies);
                    response = await _httpClient.SendAsync(req, token).DonotCapture();
                    responses.Add(response);
                    responseItem.RedirectUris.Add(response.RequestMessage.RequestUri);

                    if (requestItem.ReadResultCookie)
                        ReadCookies(response, _cookieContainer);
                }
                responseItem.StatusCode = response.StatusCode;

                if (requestItem.ReadResultHeader)
                    ReadHeader(response, responseItem);

                // response.EnsureSuccessStatusCode();

                if (requestItem.ReadResultContent)
                {
                    var contentType = response.Content.Headers.ContentType;
                    responseItem.ResponseChartSet = contentType?.CharSet;
                    if (!requestItem.ResultChartSet.IsNullOrEmpty())
                    {
                        if (contentType == null)
                        {
                            response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(requestItem.ContentType);
                            contentType = response.Content.Headers.ContentType;
                        }
                        contentType.CharSet = requestItem.ResultChartSet;
                    }
                    await ReadContentAsync(response, responseItem).DonotCapture();
                }

                return responseItem;
            }
            finally
            {
                responses.ForEach(m => m?.Dispose());
                responses.Clear();
            }
        }

        public Cookie GetCookie(Uri uri, string name)
        {
            return _cookieContainer.GetCookies(uri)[name];
        }

        public CookieCollection GetCookies(Uri uri)
        {
            return _cookieContainer.GetCookies(uri);
        }

        public void AddCookie(Cookie cookie, Uri uri = null)
        {
            if (uri == null) _cookieContainer.Add(cookie);
            else
            {
                _cookieContainer.Add(uri, cookie);
            }
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public List<Cookie> GetAllCookies()
        {
            return _cookieContainer.GetAllCookies();
        }

        public void ClearCookies(Uri uri)
        {
            foreach (Cookie cookie in GetCookies(uri))
            {
                cookie.Expired = true;
            }
        }

        public void ClearAllCookies()
        {
            foreach (var cookie in GetAllCookies())
            {
                cookie.Expired = true;
            }
        }

        public IWebProxyExt WebProxy
        {
            get => _webProxy;
            set => SetProxy(value ?? HttpProxy.None);
        }
    }
}
