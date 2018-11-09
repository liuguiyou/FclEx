// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FclEx.Http.Core;
using FclEx.Http.Proxy;
using FclEx.Utils;
using SocksSharp;
using SocksSharp.Proxy;

namespace FclEx.Http.HttpClientExt
{
    public class DefaultHttpMessageHandlerBuilder : HttpMessageHandlerBuilder
    {
        public DefaultHttpMessageHandlerBuilder(IWebProxyExt proxy)
        {
            Check.NotNull(proxy, nameof(proxy));
            PrimaryHandler = Create(proxy);
        }

        protected static HttpMessageHandler Create(IWebProxyExt proxy)
        {
            switch (proxy.Type)
            {
                case ProxyType.None: return CreateHttpClientHandler(null);

                case ProxyType.Http:
                case ProxyType.Https:
                    return CreateHttpClientHandler(proxy);

                case ProxyType.Socks5:
                    return new ProxyClientHandler<Socks5>(new ProxySettings
                    {
                        Port = proxy.Port,
                        Host = proxy.Host,
                        Credentials = proxy.Credentials as NetworkCredential
                    });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static HttpClientHandler CreateHttpClientHandler(IWebProxy proxy)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
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
            return handler;
        }

        public override HttpMessageHandler PrimaryHandler { get; }
        public override IList<DelegatingHandler> AdditionalHandlers { get; set; } = new List<DelegatingHandler>();
        public override HttpMessageHandler Build()
        {
            Check.NotNull(PrimaryHandler, nameof(PrimaryHandler));
            return CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
        }
    }
}
