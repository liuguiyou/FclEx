using System.Net.Http.Headers;

namespace FclEx.Http.SocksUtil.Http
{
    public class HttpResponseContentHeaders
	{
		public HttpResponseHeaders ResponseHeaders { get; set; }
		public HttpContentHeaders ContentHeaders { get; set; }
	}
}
