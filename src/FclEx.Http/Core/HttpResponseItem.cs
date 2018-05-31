using System;
using System.Collections.Generic;
using System.Net;
using FclEx.Collections;

namespace FclEx.Http.Core
{
    public class HttpResponseItem
    {
        private List<Uri> _redirectUris;
        private MultiValueDictionary<string, string> _headers;

        public bool HasError { get; internal set; }
        public HttpRequestItem RequestItem {get; internal set; }
        public string ResponseString { get; internal set; }
        public byte[] ResponseBytes {  get; internal set; }
        public string ResponseChartSet { get; internal set; }

        public MultiValueDictionary<string, string> Headers => _headers ?? (_headers = new MultiValueDictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        public string Location => Headers.GetFirstOrDefault(HttpConstants.Location);
        public HttpStatusCode StatusCode {  get; internal set; }

        public List<Uri> RedirectUris => _redirectUris ?? (_redirectUris = new List<Uri>());

        public static HttpResponseItem ErrorResponse { get; } = new HttpResponseItem { HasError = true };
    }
}
