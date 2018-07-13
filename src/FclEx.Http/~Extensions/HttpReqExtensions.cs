using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FclEx.Http.Core;
using FclEx.Http.Services;
using MoreLinq;

namespace FclEx.Http
{
    public static class HttpReqExtensions
    {
        public static HttpReq AddQueryValue(this HttpReq req, string key, object value) => req.AddQueryValue(key, value.ToStringSafely());

        public static HttpReq AddQueryValue(this HttpReq req, KeyValuePair<string, string> pair) => req.AddQueryValue(pair.Key, pair.Value);

        public static HttpReq AddQueryValue(this HttpReq req, Tuple<string, string> pair) => req.AddQueryValue(pair.Item1, pair.Item2);

        public static HttpReq AddQueryValue(this HttpReq req, (string, string) pair) => req.AddQueryValue(pair.Item1, pair.Item2);

        public static HttpReq AddQueryValue(this HttpReq req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddQueryValue(m));
            return req;
        }

        public static HttpReq AddQueryPair(this HttpReq req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            return req.AddQueryValue(pair[0], pair.Length > 1 ? pair[1] : "");
        }

        public static HttpReq AddFormValue(this HttpReq req, string key, object value) => req.AddFormValue(key, value.ToStringSafely());

        public static HttpReq AddFormValue(this HttpReq req, KeyValuePair<string, string> pair) => req.AddFormValue(pair.Key, pair.Value);

        public static HttpReq AddFormValue(this HttpReq req, Tuple<string, string> pair) => req.AddFormValue(pair.Item1, pair.Item2);

        public static HttpReq AddFormValue(this HttpReq req, (string, string) pair) => req.AddFormValue(pair.Item1, pair.Item2);

        public static HttpReq AddFormValue(this HttpReq req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddFormValue(m));
            return req;
        }

        public static HttpReq AddFormPair(this HttpReq req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            return req.AddFormValue(pair[0], pair.Length > 1 ? pair[1] : "");
        }

        public static HttpReq AddHeader(this HttpReq req, IEnumerable<KeyValuePair<string, string>> paras)
        {
            paras?.ForEach(m => req.AddHeader(m));
            return req;
        }

        public static HttpReq AddHeader(this HttpReq req, KeyValuePair<string, string> pair) => req.AddHeader(pair.Key, pair.Value);

        public static HttpReq AddHeaderPair(this HttpReq req, string queryPair, char sepetator = ':')
        {
            var pair = queryPair.Split(sepetator);
            req.AddHeader(pair[0], pair.Length > 1 ? pair[1] : "");
            return req;
        }

        public static HttpReq AddDataIfNotEmpty(this HttpReq req, string key, string value)
        {
            return AddDataIf(req, !value.IsNullOrEmpty(), key, value);
        }

        public static HttpReq AddDataIf(this HttpReq req, bool condition, string key, string value)
        {
            return condition ? AddData(req, key, value) : req;
        }

        public static HttpReq AddData(this HttpReq req, string key, string value)
        {
            return req.Method == HttpMethodType.Get
                ? req.AddQueryValue(key, value)
                : req.AddFormValue(key, value);
        }

        public static HttpReq AddData<T>(this HttpReq req, string key, T value)
        {
            return AddData(req, key, value.ToStringSafely());
        }

        public static HttpReq AddData(this HttpReq req,
            IEnumerable<KeyValuePair<string, string>> paras)
        {
            return req.Method == HttpMethodType.Get
                ? req.AddQueryValue(paras)
                : req.AddFormValue(paras);
        }

        public static HttpReq AcceptUtf8(this HttpReq req)
        {
            return req.AddHeader("Accept-Charset", "utf-8");
        }

        public static HttpReq AcceptCn(this HttpReq req)
        {
            return req.AddHeader("Accept-Language", "zh-CN,zh;q=0.8");
        }

        public static HttpReq Ajax(this HttpReq req)
        {
            return req.AddHeader("X-Requested-With", "XMLHttpRequest");
        }

        public static HttpReq Compress(this HttpReq req) => req.AddHeader(HttpConstants.AcceptEncoding, "gzip, deflate");

        public static HttpReq Referrer(this HttpReq req, string referrer)
        {
            req.Referrer = referrer;
            return req;
        }

        public static HttpReq TryReferrer(this HttpReq req, string referrer)
        {
            req.Referrer = req.Referrer ?? referrer;
            return req;
        }

        public static HttpReq UserAgent(this HttpReq req, string userAgent)
        {
            req.UserAgent = userAgent;
            return req;
        }

        public static HttpReq TryUserAgent(this HttpReq req, string userAgent)
        {
            req.UserAgent = req.UserAgent ?? userAgent;
            return req;
        }

        public static HttpReq ChartSet(this HttpReq req, string chartSet)
        {
            req.ResultChartSet = chartSet;
            return req;
        }

        public static HttpReq TryChartSet(this HttpReq req, string chartSet)
        {
            req.ResultChartSet = req.ResultChartSet ?? chartSet;
            return req;
        }

        public static HttpReq Timeout(this HttpReq req, int? timeout)
        {
            req.Timeout = timeout;
            return req;
        }

        public static HttpReq TryTimeout(this HttpReq req, int? timeout)
        {
            req.Timeout = req.Timeout ?? timeout;
            return req;
        }

        public static HttpReq SetData(this HttpReq req, string data)
        {
            req.StringData = data;
            return req;
        }

        public static HttpReq SetData(this HttpReq req, byte[] data)
        {
            req.ByteArrayData = data;
            return req;
        }

        public static HttpReq AddFile(this HttpReq req, HttpFileInfo file, byte[] fileBytes)
        {
            req.FileMap[file] = fileBytes;
            return req;
        }

        public static HttpReq ReadResultCookie(this HttpReq req, bool read)
        {
            req.ReadResultCookie = read;
            return req;
        }

        public static HttpReq ReadResultHeader(this HttpReq req, bool read)
        {
            req.ReadResultHeader = read;
            return req;
        }

        public static HttpReq ReadResultContent(this HttpReq req, bool read)
        {
            req.ReadResultContent = read;
            return req;
        }

        public static HttpReq Host(this HttpReq req, string host)
        {
            req.Host = host;
            return req;
        }

        public static string GetRequestHeader(this HttpReq req, string cookieHeader)
        {
            var sb = new StringBuilder();
            foreach (var pair in req.HeaderMap)
            {
                sb.AppendLine($"{pair.Key}: { pair.Value}");
            }
            sb.Append("Cookie: " + cookieHeader);
            return sb.ToString();
        }

        public static string GetRequestHeader(this HttpReq req, CookieCollection cookies)
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

        public static async ValueTask<HttpRes> SendAsync(this HttpReq req, int retryTimes = 3)
        {
            using (var http = new LightHttpService(useCookie: false))
            {
                return await http.SendAsync(req, retryTimes).DonotCapture();
            }
        }
    }
}
