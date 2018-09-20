using System.Net;
using FclEx.Http.Core;

namespace FclEx.Http
{
    public static class HttpResExtensions
    {
        public static HttpRes EnsureSuccessStatusCode(this HttpRes res)
        {
            if (res.StatusCode != HttpStatusCode.Created
                && res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"call {res.Req.GetUrl()} return unsuccessful code: {res.StatusCode}/{res.StatusCode.ToInt()}");
            }
            return res;
        }

        public static HttpRes ThrowIfError(this HttpRes res)
        {
            if (res.HasError) res.Exception.ReThrow();
            return res;
        }
    }
}
