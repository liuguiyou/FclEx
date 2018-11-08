using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FclEx.Http.Proxy;

namespace FclEx.Http.HttpClientExt
{
    public interface IHttpMessageHandlerFactory
    {
        HttpMessageHandler CreateHandler(HttpClientOptions options);
    }
}
