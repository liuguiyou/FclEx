using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace FclEx.Http
{
    public class ObjectCache
    {
        private static readonly ConcurrentDictionary<string, Exception> _exceptionDic = new ConcurrentDictionary<string, Exception>();
        private static readonly ConcurrentDictionary<string, Uri> _uriDic = new ConcurrentDictionary<string, Uri>();
        private static readonly ConcurrentDictionary<string, NameValueCollection> _urlQueryDic = new ConcurrentDictionary<string, NameValueCollection>();

        public static Exception CreateException(string msg, bool useCache = false)
        {
            return useCache ? _exceptionDic.GetOrAdd(msg, k => new Exception(k)) : new Exception(msg);
        }

        public static Uri CreateUri(string url, bool useCache = false)
        {
            return useCache
                    ? _uriDic.GetOrAdd(url, k => new Uri(k))
                    : new Uri(url);
        }

        public static NameValueCollection GetQueryPair(string url, bool useCache = false)
        {
            return useCache ? _urlQueryDic.GetOrAdd(url, HttpReqExtensions.ParseQueryStringInternal)
                : HttpReqExtensions.ParseQueryStringInternal(url);
        }
    }
}
