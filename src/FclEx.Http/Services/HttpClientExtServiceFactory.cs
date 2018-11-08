using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Http.HttpClientExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FclEx.Http.Services
{
    public class HttpClientExtServiceFactory
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HttpClientExtService> _logger;

        public HttpClientExtServiceFactory(
            IHttpClientFactory clientFactory,
            ILogger<HttpClientExtService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public HttpClientExtService Create(HttpClientOptions options = null)
        {
            options = options ?? HttpClientOptions.Default;
            return new HttpClientExtService(options, _clientFactory, _logger);
        }

        public static HttpClientExtServiceFactory Default { get; }
            = new HttpClientExtServiceFactory(DefaultHttpClientFactory.Default,
                NullLogger<HttpClientExtService>.Instance);
    }
}
