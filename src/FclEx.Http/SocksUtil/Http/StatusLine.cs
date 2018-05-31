﻿using System;
using System.Net;
using FclEx.Http.SocksUtil.Http.Extensions;
using FclEx.Http.SocksUtil.Http.Helpers;

namespace FclEx.Http.SocksUtil.Http
{
	public class StatusLine : StartLine
	{
		public HttpStatusCode StatusCode { get; private set; }

		public StatusLine(HttpProtocol protocol, HttpStatusCode status)
		{
			Protocol = protocol;
			StatusCode = status;

			StartLineString = Protocol.ToString() + Constants.SP + (int)StatusCode + Constants.SP + StatusCode.ToReasonString() + Constants.CRLF;
		}

		public static StatusLine CreateNew(string statusLineString)
		{
			try
			{
				var parts = GetParts(statusLineString);
				var protocolString = parts[0];
				var codeString = parts[1];
				var reason = parts[2];
				var protocol = new HttpProtocol(protocolString);
				var code = int.Parse(codeString);
				if (!HttpStatusCodeHelper.IsValidCode(code))
				{
					throw new NotSupportedException($"Invalid HTTP status code: {code}");
				}

				var statusCode = (HttpStatusCode)code;

				// https://tools.ietf.org/html/rfc7230#section-3.1.2
				// The reason-phrase element exists for the sole purpose of providing a
				// textual description associated with the numeric status code, mostly
				// out of deference to earlier Internet application protocols that were
				// more frequently used with interactive text clients.A client SHOULD
				// ignore the reason - phrase content.

				return new StatusLine(protocol, statusCode);
			}
			catch (Exception ex)
			{
				throw new NotSupportedException($"Invalid {nameof(StatusLine)}", ex);
			}
		}
	}
}
