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
        public bool UseDefaultProxy { get; set; } = false;
        private readonly UriBuilder _uriBuilder;
        public Uri Uri => _uriBuilder.Uri;
        public string StringData { get; set; }
        public byte[] ByteArrayData { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public HttpMethodType Method { get; set; }
        public int? Timeout { get; set; } = 5 * 1000;

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

        public string Boundary
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Boundary);
            set => HeaderMap[HttpConstants.Boundary] = value;
        }

        public HttpReq(Uri rawUrl, HttpMethodType method)
        {
            _uriBuilder = rawUrl.IsAbsoluteUri
                ? new UriBuilder(rawUrl)
                : new UriBuilder(Uri.UriSchemeHttp, "localhost", 80, rawUrl.ToString());

            if (!_uriBuilder.Query.IsNullOrEmpty())
            {
                var dic = _uriBuilder.Query.ParseQueryString();
                foreach (string key in dic)
                {
                    if (key != null)
                        QueryMap.Add(key, dic[key]);
                }
                _uriBuilder.Query = string.Empty;
            }
            ContentType = method == HttpMethodType.Get ? HttpConstants.DefaultGetContentType : HttpConstants.DefaultPostContentType;
            Method = method;
            AddHeader(HttpConstants.UserAgent, HttpConstants.DefaultUserAgent);
        }

        public HttpReq(HttpMethodType method, string rawUrl)
            : this(rawUrl, method) { }

        public HttpReq(string rawUrl, HttpMethodType method)
            : this(new Uri(rawUrl, UriKind.RelativeOrAbsolute), method) { }

        public static HttpReq Json(Uri url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.JsonContentType };
        public static HttpReq Json(string url) => Json(new Uri(url, UriKind.RelativeOrAbsolute));

        public static HttpReq Form(Uri url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.FormContentType };
        public static HttpReq Form(string url) => Form(new Uri(url, UriKind.RelativeOrAbsolute));

        public static HttpReq Get(Uri url) => new HttpReq(url, HttpMethodType.Get) { ContentType = HttpConstants.DefaultGetContentType };
        public static HttpReq Get(string url) => Get(new Uri(url, UriKind.RelativeOrAbsolute));

        public static HttpReq Upload(Uri url) => new HttpReq(url, HttpMethodType.Post) { ContentType = HttpConstants.ByteArrayContentType };
        public static HttpReq Upload(string url) => Upload(new Uri(url, UriKind.RelativeOrAbsolute));

        public static HttpReq MultiPart(Uri url) => new HttpReq(url, HttpMethodType.Post)
        {
            Boundary = "----WebKitFormBoundaryImw0tVH7wlMdFALP",
            ContentType = HttpConstants.MultiPartContentType,
        };
        public static HttpReq MultiPart(string url) => MultiPart(new Uri(url, UriKind.RelativeOrAbsolute));

        public static HttpReq Create(Uri url, HttpReqType reqType)
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
        public static HttpReq Create(string url, HttpReqType reqType) => Create(new Uri(url, UriKind.RelativeOrAbsolute), reqType);

        public string Fragment
        {
            get => _uriBuilder.Fragment;
            set => _uriBuilder.Fragment = value;
        }
        public string Host
        {
            get => _uriBuilder.Host;
            set
            {
                var m = CommonRegex.HostPort.Match(value);
                if (!m.Success) m = CommonRegex.Ipv6HostPort.Match(value);
                if (m.Success)
                {
                    var h = m.Groups[1].Value;
                    var p = m.TryGetInt(2, 80);
                    if (h != Host || p != Port)
                    {
                        _uriBuilder.Host = h;
                        _uriBuilder.Port = p;
                    }
                }
                else _uriBuilder.Host = value;
            }
        }

        public string UserName
        {
            get => _uriBuilder.UserName;
            set => _uriBuilder.UserName = value;
        }
        public string Password
        {
            get => _uriBuilder.Password;
            set => _uriBuilder.Password = value;
        }
        public string Path
        {
            get => _uriBuilder.Path;
            set => _uriBuilder.Path = value;
        }
        public int Port
        {
            get => _uriBuilder.Port;
            set => _uriBuilder.Port = value;
        }
        public string Scheme
        {
            get => _uriBuilder.Scheme;
            set => _uriBuilder.Scheme = value;
        }

        private bool HasQuery => !_queryMap.IsNullOrEmpty();

        public string GetUrl()
        {
            _uriBuilder.Query = QueryMap.Select(m => $"{m.Key.UrlEncode()}={m.Value.UrlEncode()}").JoinWith("&");
            var url = Uri.ToString();
            _uriBuilder.Query = string.Empty;
            return url;
        }

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

