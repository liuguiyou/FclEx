using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace FclEx.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static bool IfRedirect(this HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Redirect
                   || response.StatusCode == HttpStatusCode.Moved
                   || response.StatusCode == HttpStatusCode.SeeOther
                   || response.StatusCode == HttpStatusCode.RedirectKeepVerb;
        }

        public static Uri GetRedirectUri(this HttpResponseMessage response)
        {
            var uri = response.Headers.Location;
            if (!uri.IsAbsoluteUri)
                uri = new Uri(response.RequestMessage.RequestUri, uri);
            return uri;
        }
    }
}
