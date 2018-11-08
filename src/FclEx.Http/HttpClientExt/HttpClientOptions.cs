﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using FclEx.Http.Proxy;

namespace FclEx.Http.HttpClientExt
{
    public class HttpClientOptions
    {
        internal static readonly TimeSpan _minimumHandlerLifetime = TimeSpan.FromSeconds(1);

        private TimeSpan _handlerLifetime = TimeSpan.FromMinutes(2);
        private IWebProxyExt _proxy = WebProxyExt.None;

        public TimeSpan HandlerLifetime
        {
            get => _handlerLifetime;
            set
            {
                if (value != Timeout.InfiniteTimeSpan && value < _minimumHandlerLifetime)
                {
                    throw new ArgumentException("The handler lifetime must be at least 1 second.", nameof(value));
                }

                _handlerLifetime = value;
            }
        }

        public string Name { get; set; } = string.Empty;

        public IWebProxyExt Proxy
        {
            get => _proxy;
            set => _proxy = value ?? WebProxyExt.None;
        }

        public bool UseCookie { get; set; } = true;

        public static HttpClientOptions Default { get; } = new HttpClientOptions();
    }
}
