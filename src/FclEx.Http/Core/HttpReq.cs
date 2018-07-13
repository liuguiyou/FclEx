using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FclEx.Utils;

namespace FclEx.Http.Core
{
    public class HttpReq
    {
        public Uri RawUri { get; private set; }
        public string RawUrl => RawUri.ToString();
        public string StringData { get; set; }
        public byte[] ByteArrayData { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public HttpMethodType Method { get; set; }
        public int? Timeout { get; set; } = 10 * 1000;

        public string ResultChartSet { get; set; }
        public HttpResultType ResultType { get; set; }
        public bool ReadResultCookie { get; set; } = true;
        public bool ReadResultHeader { get; set; } = true;
        public bool ReadResultContent { get; set; } = true;

        public Dictionary<string, string> HeaderMap { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, string> _queryMap;
        public Dictionary<string, string> QueryMap => _queryMap ?? (_queryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        private Dictionary<string, string> _formMap;
        public Dictionary<string, string> FormMap => _formMap ?? (_formMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        private Dictionary<HttpFileInfo, byte[]> _fileMap;
        public Dictionary<HttpFileInfo, byte[]> FileMap => _fileMap ?? (_fileMap = new Dictionary<HttpFileInfo, byte[]>());

        public string ContentType
        {
            get
            {
                var type = HeaderMap.GetOrDefault(HttpConstants.ContentType);
                return type == HttpConstants.MultiPartContentType
                    ? "multipart/form-data; boundary=" + Boundary
                    : type;
            }
            set => HeaderMap[HttpConstants.ContentType] = value;
        }

        public string Referrer
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Referrer);
            set => HeaderMap[HttpConstants.Referrer] = value;
        }

        public string Origin
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Origin);
            set => HeaderMap[HttpConstants.Origin] = value;
        }

        public string UserAgent
        {
            get => HeaderMap.GetOrDefault(HttpConstants.UserAgent);
            set => HeaderMap[HttpConstants.UserAgent] = value;
        }

        public string Host
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Host);
            set
            {
                var m = CommonRegex.HostPort.Match(value);
                if(!m.Success) throw new ArgumentException("Not a valid host: " + value);

                var h = m.Groups[1].Value;
                var p = m.TryGetInt(2, 80);

                if (h != RawUri.Host || p != RawUri.Port)
                {
                    HeaderMap[HttpConstants.Host] = value;
                    var uriBuilder = new UriBuilder(RawUri.Scheme, h, p, RawUri.LocalPath);
                    RawUri = uriBuilder.Uri;
                }
            }
        }

        public string Boundary
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Boundary);
            set => HeaderMap[HttpConstants.Boundary] = value;
        }

        public HttpReq(Uri rawUrl, HttpMethodType method) : this(rawUrl.ToString(), method) { }

        public HttpReq(HttpMethodType method, string rawUrl) : this(rawUrl, method) { }

        public HttpReq(string rawUrl, HttpMethodType method)
        {
            var parts = rawUrl.Split('?');
            RawUri = new Uri(parts[0]);
            if (parts.Length > 1)
            {
                var dic = parts[1].ParseQueryString();
                foreach (string key in dic)
                {
                    if (key != null)
                        QueryMap.Add(key, dic[key]);
                }
            }
            ContentType = method == HttpMethodType.Get ? HttpConstants.DefaultGetContentType : HttpConstants.DefaultPostContentType;
            Method = method;
            AddHeader(HttpConstants.Host, RawUri.Authority);
            AddHeader(HttpConstants.UserAgent, HttpConstants.DefaultUserAgent);
        }

        public static HttpReq Json(string url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.JsonContentType };
        public static HttpReq Json(Uri url) => Json(url.ToString());

        public static HttpReq Form(string url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.FormContentType };
        public static HttpReq Form(Uri url) => Form(url.ToString());

        public static HttpReq Get(string url) => new HttpReq(url, HttpMethodType.Get) { ContentType = HttpConstants.DefaultGetContentType };
        public static HttpReq Get(Uri url) => Get(url.ToString());

        public static HttpReq Upload(string url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.ByteArrayContentType };
        public static HttpReq Upload(Uri url) => Upload(url.ToString());

        public static HttpReq MultiPart(string url) => new HttpReq(url, HttpMethodType.Post)
        {
            Boundary = "----WebKitFormBoundaryImw0tVH7wlMdFALP",
            ContentType = HttpConstants.MultiPartContentType,
        };

        public static HttpReq MultiPart(Uri url) => MultiPart(url.ToString());

        public static HttpReq Create(string url, HttpReqType reqType)
        {
            switch (reqType)
            {
                case HttpReqType.Get: return Get(url);
                case HttpReqType.Form: return Form(url);
                case HttpReqType.Json: return Json(url);
                case HttpReqType.Upload: return Upload(url);
                case HttpReqType.MultiPart: return MultiPart(url);
                default: throw new ArgumentOutOfRangeException(nameof(reqType), reqType, null);
            }
        }

        public bool HasQuery => !_queryMap.IsNullOrEmpty();

        public string GetUrl() => !HasQuery ? RawUrl : $"{RawUrl}?{QueryMap.Select(m => $"{m.Key.UrlEncode()}={m.Value.UrlEncode()}").JoinWith("&")}";

        public HttpReq AddQueryValue(string key, string value)
        {
            QueryMap[key] = value ?? "";
            return this;
        }

        public HttpReq AddFormValue(string key, string value)
        {
            FormMap[key] = value ?? "";
            return this;
        }

        public HttpReq AddHeader(string key, string value)
        {
            HeaderMap[key] = value ?? "";
            return this;
        }

        public HttpReq TryAddHeader(string key, string value)
        {
            if (!HeaderMap.ContainsKey(key))
                HeaderMap[key] = value ?? "";
            return this;
        }

        public byte[] GetBinaryData()
        {
            var type = HeaderMap.GetOrDefault(HttpConstants.ContentType);
            switch (type)
            {
                case HttpConstants.JsonContentType: return StringData.ToBytes(Encoding);

                case HttpConstants.FormContentType: return FormMap.ToQueryString().ToBytes(Encoding);

                case HttpConstants.MultiPartContentType:
                    {
                        using (var mem = new MemoryStream())
                        {
                            var sb = new StringBuilder(1024);
                            // Write the values
                            foreach (var pair in FormMap)
                            {
                                sb.AppendHttpLine(HttpConstants.EncapsulationBoundary + Boundary);
                                sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"{1}{1}", pair.Key, HttpConstants.NewLine);
                                sb.AppendHttpLine(pair.Value);
                            }
                            sb.ToString().ToUtf8Bytes().WriteTo(mem);
                            // Write the files
                            foreach (var file in FileMap)
                            {
                                var data = new StringBuilder(192);
                                data.AppendHttpLine(HttpConstants.EncapsulationBoundary + Boundary);
                                data.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Key.Name, file.Key.FileName, HttpConstants.NewLine);
                                data.AppendFormat("Content-Type: {0}{1}{1}", file.Key.ContentType, HttpConstants.NewLine);
                                data.ToString().ToUtf8Bytes().WriteTo(mem);
                                file.Value.WriteTo(mem);
                                HttpConstants.NewLineBytes.WriteTo(mem);
                            }
                            (HttpConstants.EncapsulationBoundary + Boundary + HttpConstants.EncapsulationBoundary).ToUtf8Bytes().WriteTo(mem);
                            return mem.ToArray();
                        }
                    }

                default:
                case HttpConstants.ByteArrayContentType:
                    return ByteArrayData ?? Array.Empty<byte>();
            }
        }
    }
}
