using System.Net.Http.Headers;

namespace FclEx.Http.SocksUtil.Http
{
	public class HttpRequestContentHeaders
	{
		public HttpRequestHeaders RequestHeaders { get; set; }
		public HttpContentHeaders ContentHeaders { get; set; }
	}
}
