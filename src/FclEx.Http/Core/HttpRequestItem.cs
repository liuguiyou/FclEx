using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FclEx.Http.Core
{
    public struct HttpFileInfo
    {
        public HttpFileInfo(string name, string fileName, string contentType)
        {
            Name = name;
            FileName = fileName;
            ContentType = contentType;
        }

        public string Name { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }

    public class HttpRequestItem
    {
        public Uri RawUri { get; }
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
            set => HeaderMap[HttpConstants.Host] = value;
        }

        public string Boundary
        {
            get => HeaderMap.GetOrDefault(HttpConstants.Boundary);
            set => HeaderMap[HttpConstants.Boundary] = value;
        }

        public HttpRequestItem(Uri rawUrl, HttpMethodType method) : this(rawUrl.ToString(), method) { }

        public HttpRequestItem(HttpMethodType method, string rawUrl) : this(rawUrl, method) { }

        public HttpRequestItem(string rawUrl, HttpMethodType method)
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
            AddHeader(HttpConstants.Host, RawUri.Host);
            AddHeader(HttpConstants.UserAgent, HttpConstants.DefaultUserAgent);
        }

        public static HttpRequestItem CreateJsonRequest(string url) => new HttpRequestItem(url, HttpMethodType.Post) { ContentType = HttpConstants.JsonContentType };
        public static HttpRequestItem CreateJsonRequest(Uri url) => CreateJsonRequest(url.ToString());

        public static HttpRequestItem CreateFormRequest(string url) => new HttpRequestItem(url, HttpMethodType.Post) { ContentType = HttpConstants.FormContentType };
        public static HttpRequestItem CreateFormRequest(Uri url) => CreateFormRequest(url.ToString());

        public static HttpRequestItem CreateGetRequest(string url) => new HttpRequestItem(url, HttpMethodType.Get) { ContentType = HttpConstants.DefaultGetContentType };
        public static HttpRequestItem CreateGetRequest(Uri url) => CreateGetRequest(url.ToString());

        public static HttpRequestItem CreateUploadRequest(string url) => new HttpRequestItem(url, HttpMethodType.Post) { ContentType = HttpConstants.ByteArrayContentType };
        public static HttpRequestItem CreateUploadRequest(Uri url) => CreateUploadRequest(url.ToString());

        public static HttpRequestItem CreateMultiPartRequest(string url) => new HttpRequestItem(url, HttpMethodType.Post)
        {
            Boundary = "----WebKitFormBoundaryImw0tVH7wlMdFALP",
            ContentType = HttpConstants.MultiPartContentType,
        };

        public static HttpRequestItem CreateMultiPartRequest(Uri url) => CreateMultiPartRequest(url.ToString());

        public static HttpRequestItem CreateRequest(string url, EnumRequestType requestType)
        {
            switch (requestType)
            {
                case EnumRequestType.Get: return CreateGetRequest(url);
                case EnumRequestType.Form: return CreateFormRequest(url);
                case EnumRequestType.Json: return CreateJsonRequest(url);
                case EnumRequestType.Upload: return CreateUploadRequest(url);
                case EnumRequestType.MultiPart: return CreateMultiPartRequest(url);
                default: throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
            }
        }

        public bool HasQuery => !_queryMap.IsNullOrEmpty();

        public string GetUrl() => !HasQuery ? RawUrl : $"{RawUrl}?{QueryMap.Select(m => $"{m.Key.UrlEncode()}={m.Value.UrlEncode()}").JoinWith("&")}";

        public HttpRequestItem AddQueryValue(string key, string value)
        {
            QueryMap[key] = value ?? "";
            return this;
        }

        public HttpRequestItem AddFormValue(string key, string value)
        {
            FormMap[key] = value ?? "";
            return this;
        }

        public HttpRequestItem AddHeader(string key, string value)
        {
            HeaderMap[key] = value ?? "";
            return this;
        }

        public HttpRequestItem TryAddHeader(string key, string value)
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
