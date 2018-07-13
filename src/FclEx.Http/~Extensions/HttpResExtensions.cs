using System.Net;
using FclEx.Http.Core;

namespace FclEx.Http
{
    public static class HttpResExtensions
    {
        public static HttpRes EnsureSuccessStatusCode(this HttpRes httpResponse)
        {
            if (httpResponse.StatusCode != HttpStatusCode.Created
                && httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"call {httpResponse.Req.GetUrl()} return unsuccessful code: {httpResponse.StatusCode}/{httpResponse.StatusCode.ToInt()}");
            }
            return httpResponse;
        }
    }
}
