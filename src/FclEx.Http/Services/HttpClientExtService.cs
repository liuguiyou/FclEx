﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.Core;
using FclEx.Http.HttpClientExt;
using FclEx.Http.Proxy;
using FclEx.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Http.Services
{
    public class HttpClientExtService : AbstractHttpClientService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClientOptions _options;

        public static TimerLazy<HttpClientExtService> Default { get; } = new TimerLazy<HttpClientExtService>(() =>
                new HttpClientExtService(null, false),
            LazyThreadSafetyMode.ExecutionAndPublication,
            TimeSpan.FromMinutes(2));

        public override ValueTask<HttpRes> ExecuteAsync(HttpReq httpReq, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var httpClient = _clientFactory.CreateClient(_options);
            return ExecuteAsync(httpClient, httpReq, token);
        }

        public HttpClientExtService(
            IWebProxyExt proxy = null,
            bool useCookie = true)
            : this(new HttpClientOptions { Proxy = proxy, UseCookie = useCookie },
                DefaultHttpClientFactory.Default,
                NullLogger<HttpClientExtService>.Instance)
        {
        }

        protected override void SetProxy(IWebProxyExt proxy)
        {
            base.SetProxy(proxy);
            if (_options != null)
                _options.Proxy = _webProxy;
        }

        public HttpClientExtService(
            HttpClientOptions options,
            IHttpClientFactory clientFactory,
            ILogger<HttpClientExtService> logger)
            : base(options.UseCookie, options.Proxy, logger)
        {
            _clientFactory = clientFactory;
            _options = Check.NotNull(options, nameof(options));
        }
    }
}
