using System.Net;
using System.Net.Http;

namespace FclEx.Http.SocksUtil.Http.Extensions
{
	public static class HttpStatusCodeExtensions
	{
		public static string ToReasonString(this HttpStatusCode me)
		{
			var message = new HttpResponseMessage(me);
			return message.ReasonPhrase;
		}
	}
}