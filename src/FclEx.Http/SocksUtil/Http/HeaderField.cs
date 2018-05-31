using System;
using System.IO;
using FclEx.Http.SocksUtil.Http.Extensions;

namespace FclEx.Http.SocksUtil.Http
{
    public class HeaderField
    {
		public string Name { get; private set; }
		public string Value { get; private set; }

		public HeaderField(string name, string value)
		{
			Name = name;

			value = CorrectObsFolding(value);
			Value = value;
		}

		public static string CorrectObsFolding(string text)
		{
			// fix obs line folding
			// https://tools.ietf.org/html/rfc7230#section-3.2.4
			// replace each received obs-fold with one or more SP octets prior to interpreting the field value
			text = text.Replace(Constants.CRLF + Constants.SP, Constants.SP + Constants.SP);
			text = text.Replace(Constants.CRLF + Constants.HTAB, Constants.SP + Constants.HTAB);
			return text;
		}

		// https://tools.ietf.org/html/rfc7230#section-3.2
		// Each header field consists of a case-insensitive field name followed
		// by a colon(":"), optional leading whitespace, the field value, and
		// optional trailing whitespace.
		// header-field   = field-name ":" OWS field-value OWS
		// The OWS rule is used where zero or more linear whitespace octets	might appear.
		public string ToString(bool endWithCRLF)
		{
			var ret = Name + ":" + Value;
			if (endWithCRLF)
			{
				ret += Constants.CRLF;
			}
			return ret;
		}

		public override string ToString()
		{
			return ToString(true);
		}

		public static HeaderField CreateNew(string fieldString)
		{
			fieldString = fieldString.TrimEnd(Constants.CRLF, StringComparison.Ordinal);

			using(var reader = new StringReader(fieldString))
			{
				var name = reader.ReadPart(':');
				// if empty
				if(name == null || name.Trim() == "") throw new FormatException($"Wrong {nameof(HeaderField)}: {fieldString}");
				// https://tools.ietf.org/html/rfc7230#section-3.2.4
				// No whitespace is allowed between the header field-name and colon.
				// A proxy MUST remove any such whitespace from a response message before forwarding the message downstream.
				name = name.TrimEnd();
				// whitespace not allowed
				if (name != name.Trim()) throw new FormatException($"Wrong {nameof(HeaderField)}: {fieldString}");

				var value = reader.ReadToEnd();
				// correction
				if (value == null) value = "";
				value = value.Trim(); // better to use without whitespaces


				return new HeaderField(name, value);
			}
		}
	}
}
