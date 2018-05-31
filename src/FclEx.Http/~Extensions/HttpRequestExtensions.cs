using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using FclEx.Http.Core;
using MoreLinq;

namespace FclEx.Http
{
    public static class HttpRequestExtensions
    {
        public static HttpRequestItem AddQueryValue(this HttpRequestItem req, string key, object value) => req.AddQueryValue(key, value.ToStringSafely());

        public static HttpRequestItem AddQueryValue(this HttpRequestItem req, KeyValuePair<string, string> pair) => req.AddQueryValue(pair.Key, pair.Value);

        public static HttpRequestItem AddQueryValue(this HttpRequestItem req, Tuple<string, string> pair) => req.AddQueryValue(pair.Item1, pair.Item2);

        public static HttpRequestItem AddQueryValue(this HttpRequestItem req, (string, string) pair) => req.AddQueryValue(pair.Item1, pair.Item2);

        public static HttpRequestItem AddQueryValue(this HttpRequestItem req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddQueryValue(m));
            return req;
        }

        public static HttpRequestItem AddQueryPair(this HttpRequestItem req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            return req.AddQueryValue(pair[0], pair.Length > 1 ? pair[1] : "");
        }

        public static HttpRequestItem AddFormValue(this HttpRequestItem req, string key, object value) => req.AddFormValue(key, value.ToStringSafely());

        public static HttpRequestItem AddFormValue(this HttpRequestItem req, KeyValuePair<string, string> pair) => req.AddFormValue(pair.Key, pair.Value);

        public static HttpRequestItem AddFormValue(this HttpRequestItem req, Tuple<string, string> pair) => req.AddFormValue(pair.Item1, pair.Item2);

        public static HttpRequestItem AddFormValue(this HttpRequestItem req, (string, string) pair) => req.AddFormValue(pair.Item1, pair.Item2);

        public static HttpRequestItem AddFormValue(this HttpRequestItem req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddFormValue(m));
            return req;
        }

        public static HttpRequestItem AddFormPair(this HttpRequestItem req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            return req.AddFormValue(pair[0], pair.Length > 1 ? pair[1] : "");
        }

        public static HttpRequestItem AddHeader(this HttpRequestItem req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddHeader(m));
            return req;
        }

        public static HttpRequestItem AddHeader(this HttpRequestItem req, KeyValuePair<string, string> pair) => req.AddHeader(pair.Key, pair.Value);

        public static HttpRequestItem AddHeaderPair(this HttpRequestItem req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            req.AddHeader(pair[0], pair.Length > 1 ? pair[1] : "");
            return req;
        }

        public static HttpRequestItem AddData(this HttpRequestItem req, string key, string value)
        {
            return req.Method == HttpMethodType.Get
                ? req.AddQueryValue(key, value)
                : req.AddFormValue(key, value);
        }

        public static HttpRequestItem AddData<T>(this HttpRequestItem req, string key, T value)
        {
            var str = value == null ? "" : value.ToString();
            return req.Method == HttpMethodType.Get
                ? req.AddQueryValue(key, str)
                : req.AddFormValue(key, str);
        }

        public static HttpRequestItem AddData(this HttpRequestItem req,
            IEnumerable<KeyValuePair<string, string>> paras)
        {
            return req.Method == HttpMethodType.Get
                ? req.AddQueryValue(paras)
                : req.AddFormValue(paras);
        }

        public static HttpRequestItem AcceptUtf8(this HttpRequestItem req)
        {
            return req.AddHeader("Accept-Charset", "utf-8");
        }

        public static HttpRequestItem AcceptCn(this HttpRequestItem req)
        {
            return req.AddHeader("Accept-Language", "zh-CN,zh;q=0.8");
        }

        public static HttpRequestItem Ajax(this HttpRequestItem req)
        {
            return req.AddHeader("X-Requested-With", "XMLHttpRequest");
        }

        public static HttpRequestItem Compress(this HttpRequestItem req) => req.AddHeader(HttpConstants.AcceptEncoding, "gzip, deflate");

        public static HttpRequestItem Referrer(this HttpRequestItem req, string referrer)
        {
            req.Referrer = referrer;
            return req;
        }

        public static HttpRequestItem TryReferrer(this HttpRequestItem req, string referrer)
        {
            req.Referrer = req.Referrer ?? referrer;
            return req;
        }

        public static HttpRequestItem UserAgent(this HttpRequestItem req, string userAgent)
        {
            req.UserAgent = userAgent;
            return req;
        }

        public static HttpRequestItem TryUserAgent(this HttpRequestItem req, string userAgent)
        {
            req.UserAgent = req.UserAgent ?? userAgent;
            return req;
        }

        public static HttpRequestItem ChartSet(this HttpRequestItem req, string chartSet)
        {
            req.ResultChartSet = chartSet;
            return req;
        }

        public static HttpRequestItem TryChartSet(this HttpRequestItem req, string chartSet)
        {
            req.ResultChartSet = req.ResultChartSet ?? chartSet;
            return req;
        }

        public static HttpRequestItem Timeout(this HttpRequestItem req, int timeout)
        {
            req.Timeout = timeout;
            return req;
        }

        public static HttpRequestItem TryTimeout(this HttpRequestItem req, int timeout)
        {
            req.Timeout = req.Timeout ?? timeout;
            return req;
        }

        public static HttpRequestItem SetData(this HttpRequestItem req, string data)
        {
            req.StringData = data;
            return req;
        }

        public static HttpRequestItem SetData(this HttpRequestItem req, byte[] data)
        {
            req.ByteArrayData = data;
            return req;
        }

        public static HttpRequestItem AddFile(this HttpRequestItem req, HttpFileInfo file, byte[] fileBytes)
        {
            req.FileMap[file] = fileBytes;
            return req;
        }

        public static HttpRequestItem ReadResultCookie(this HttpRequestItem req, bool read)
        {
            req.ReadResultCookie = read;
            return req;
        }

        public static HttpRequestItem ReadResultHeader(this HttpRequestItem req, bool read)
        {
            req.ReadResultHeader = read;
            return req;
        }

        public static HttpRequestItem ReadResultContent(this HttpRequestItem req, bool read)
        {
            req.ReadResultContent = read;
            return req;
        }

        public static string GetRequestHeader(this HttpRequestItem req, string cookieHeader)
        {
            var sb = new StringBuilder();
            foreach (var pair in req.HeaderMap)
            {
                sb.AppendLine($"{pair.Key}: { pair.Value}");
            }
            sb.AppendLine("Cookie: " + cookieHeader);
            return sb.ToString();
        }

        public static string GetRequestHeader(this HttpRequestItem req, CookieCollection cookies)
        {
            return GetRequestHeader(req, cookies.OfType<Cookie>().Select(m => m.ToString()).JoinWith("; "));
        }

        internal static NameValueCollection ParseQueryStringInternal(string query)
        {
            var result = new NameValueCollection();
            if (query.Length == 0) return result;

            var decoded = query;
            var decodedLength = decoded.Length;
            var namePos = 0;
            var first = true;
            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (var q = namePos; q < decodedLength; q++)
                {
                    if ((valuePos == -1) && (decoded[q] == '='))
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '?')
                        namePos++;
                }

                string name;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = decoded.Substring(namePos, valuePos - namePos - 1).UrlDecode();
                }
                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }
                var value = decoded.Substring(valuePos, valueEnd - valuePos).UrlDecode();

                result.Add(name, value);
                if (namePos == -1)
                    break;
            }

            return result;
        }

        public static NameValueCollection ParseQueryString(this string query, bool useCache = false)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var index = query.IndexOf("?", StringComparison.Ordinal);
            if (index >= 0) query = query.Substring(index + 1);

            return ObjectCache.GetQueryPair(query, useCache);
        }


    }
}
