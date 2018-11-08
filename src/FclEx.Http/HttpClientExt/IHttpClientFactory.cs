using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FclEx.Http.HttpClientExt
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(HttpClientOptions options);
    }
}
