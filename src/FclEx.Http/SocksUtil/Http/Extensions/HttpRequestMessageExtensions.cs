﻿using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.SocksUtil.Http.Helpers;

namespace FclEx.Http.SocksUtil.Http.Extensions
{
    public static class HttpRequestMessageExtensions
    {
		public static async Task<HttpRequestMessage> CreateNewAsync(this HttpRequestMessage me, Stream requestStream, CancellationToken ctsToken = default(CancellationToken))
		{
			// https://tools.ietf.org/html/rfc7230#section-3
			// The normal procedure for parsing an HTTP message is to read the
			// start - line into a structure, read each header field into a hash table
			// by field name until the empty line, and then use the parsed data to
			// determine if a message body is expected.If a message body has been
			// indicated, then it is read as a stream until an amount of octets
			// equal to the message body length is read or the connection is closed.

			// https://tools.ietf.org/html/rfc7230#section-3
			// All HTTP/ 1.1 messages consist of a start - line followed by a sequence
			// of octets in a format similar to the Internet Message Format
			// [RFC5322]: zero or more header fields(collectively referred to as
			// the "headers" or the "header section"), an empty line indicating the
			// end of the header section, and an optional message body.
			// HTTP - message = start - line
			//					* (header - field CRLF )
			//					CRLF
			//					[message - body]
			
			var position = 0;
			string startLine = await HttpMessageHelper.ReadStartLineAsync(requestStream, ctsToken).ConfigureAwait(false);
			position += startLine.Length;

			var requestLine = RequestLine.CreateNew(startLine);
			var request = new HttpRequestMessage(requestLine.Method, requestLine.URI);

			string headers = await HttpMessageHelper.ReadHeadersAsync(requestStream, ctsToken).ConfigureAwait(false);
			position += headers.Length + 2;

			var headerSection = HeaderSection.CreateNew(headers);
			var headerStruct = headerSection.ToHttpRequestHeaders();

			HttpMessageHelper.AssertValidHeaders(headerStruct.RequestHeaders, headerStruct.ContentHeaders);
			request.Content = await HttpMessageHelper.GetContentAsync(requestStream, headerStruct, ctsToken).ConfigureAwait(false);

			HttpMessageHelper.CopyHeaders(headerStruct.RequestHeaders, request.Headers);
			if (request.Content != null)
			{
				HttpMessageHelper.CopyHeaders(headerStruct.ContentHeaders, request.Content.Headers);
			}
			return request;
		}

		public static async Task<string> ToHttpStringAsync(this HttpRequestMessage me, CancellationToken ctsToken = default(CancellationToken))
		{
			// https://tools.ietf.org/html/rfc7230#section-5.4
			// The "Host" header field in a request provides the host and port
			// information from the target URI, enabling the origin server to
			// distinguish among resources while servicing requests for multiple
			// host names on a single IP address.
			// Host = uri - host[":" port] ; Section 2.7.1
			// A client MUST send a Host header field in all HTTP/1.1 request messages.
			if (me.Method != new HttpMethod("CONNECT"))
			{
				if (!me.Headers.Contains("Host"))
				{
					// https://tools.ietf.org/html/rfc7230#section-5.4
					// If the target URI includes an authority component, then a
					// client MUST send a field-value for Host that is identical to that			   
					// authority component, excluding any userinfo subcomponent and its "@"			   
					// delimiter(Section 2.7.1).If the authority component is missing or			   
					// undefined for the target URI, then a client MUST send a Host header			   
					// field with an empty field - value.					
					me.Headers.TryAddWithoutValidation("Host", me.RequestUri.Authority);
				}
			}

			var startLine = new RequestLine(me.Method, me.RequestUri, new HttpProtocol($"HTTP/{me.Version.Major}.{me.Version.Minor}")).ToString();

			string headers = "";
			if (me.Headers != null && me.Headers.Count() != 0)
			{
				var headerSection = HeaderSection.CreateNew(me.Headers);
				headers += headerSection.ToString(endWithTwoCRLF: false);
			}
			
			string messageBody = "";
			if (me.Content != null)
			{
				if (me.Content.Headers != null && me.Content.Headers.Count() != 0)
				{
					var headerSection = HeaderSection.CreateNew(me.Content.Headers);
					headers += headerSection.ToString(endWithTwoCRLF: false);
				}

				ctsToken.ThrowIfCancellationRequested();
				messageBody = await me.Content.ReadAsStringAsync().ConfigureAwait(false);
			}

			return startLine + headers + Constants.CRLF + messageBody;
		}
	}
}
