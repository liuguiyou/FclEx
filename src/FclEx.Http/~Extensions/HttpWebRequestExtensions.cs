using System.Net;
using System.Threading.Tasks;

namespace FclEx.Http
{
    public static class HttpWebRequestExtensions
    {
        public static async Task<HttpWebResponse> GetHttpResponseAsync(this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)await req.GetResponseAsync().DonotCapture();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    return (HttpWebResponse)ex.Response;
                else throw;
            }
        }
    }
}
