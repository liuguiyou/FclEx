using System;
using System.Net;

namespace FclEx.Http
{
    public static class HttpWebResponseExtensions
    {
        public static HttpWebResponse EnsureSuccessStatusCode(this HttpWebResponse httpResponse)
        {
            if (httpResponse.StatusCode != HttpStatusCode.Created
                && httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"call {httpResponse.ResponseUri} return unsuccessful code: {httpResponse.StatusCode}/{httpResponse.StatusCode.ToInt()}");
            }
            return httpResponse;
        }

        public static Uri GetRedirectUri(this HttpWebResponse response)
        {
            var uri = new Uri(response.Headers[HttpResponseHeader.Location], UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(response.ResponseUri, uri);
            return uri;
        }

        public static bool IfRedirect(this HttpWebResponse response)
        {
            return response.StatusCode == HttpStatusCode.Redirect
                   || response.StatusCode == HttpStatusCode.Moved
                   || response.StatusCode == HttpStatusCode.SeeOther
                   || response.StatusCode == HttpStatusCode.RedirectKeepVerb;
        }
    }
}
