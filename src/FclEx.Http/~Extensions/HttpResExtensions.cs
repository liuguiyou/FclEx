using System.Net;
using System.Threading.Tasks;
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

        public static async Task<HttpRes> ThrowIfError(this Task<HttpRes> task)
        {
            var res = await task.DonotCapture();
            res.ThrowIfError();
            return res;
        }

        public static async ValueTask<HttpRes> ThrowIfError(this ValueTask<HttpRes> task)
        {
            var res = await task.DonotCapture();
            res.ThrowIfError();
            return res;
        }
    }
}
