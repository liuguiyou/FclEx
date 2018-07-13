﻿using System;
using System.Collections.Generic;
using System.Net;
using FclEx.Collections;

namespace FclEx.Http.Core
{
    public class HttpRes
    {
        private List<Uri> _redirectUris;
        private MultiValueDictionary<string, string> _headers;

        public string Location => Headers.GetFirstOrDefault(HttpConstants.Location);
        public bool HasError => Exception != null;
        public HttpReq Req { get; internal set; }
        public string ResponseString { get; internal set; }
        public byte[] ResponseBytes { get; internal set; }
        public string ResponseChartSet { get; internal set; }
        public Exception Exception { get; internal set; }
        public MultiValueDictionary<string, string> Headers => 
            _headers ?? (_headers = new MultiValueDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase));
        public HttpStatusCode StatusCode { get; internal set; }
        public List<Uri> RedirectUris => _redirectUris ?? (_redirectUris = new List<Uri>());

        public static HttpRes CreateError(Exception e) => new HttpRes { Exception = e };
    }
}
