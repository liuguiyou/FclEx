using System.Net;
using FclEx.Http.Core;

namespace FclEx.Http
{
    public static class HttpResponseExtensions
    {
        public static HttpResponseItem EnsureSuccessStatusCode(this HttpResponseItem httpResponse)
        {
            if (httpResponse.StatusCode != HttpStatusCode.Created
                && httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"call {httpResponse.RequestItem.GetUrl()} return unsuccessful code: {httpResponse.StatusCode}/{httpResponse.StatusCode.ToInt()}");
            }
            return httpResponse;
        }
    }
}
