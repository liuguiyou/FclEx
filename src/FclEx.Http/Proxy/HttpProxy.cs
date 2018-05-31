using System;
using System.Net;

namespace FclEx.Http.Proxy
{
    public sealed class HttpProxy : IWebProxyExt, IEquatable<HttpProxy>
    {
        public EnumProxyType ProxyType { get; }
        public string Host { get; }
        public int Port { get; }
        private readonly Uri _uri;
        private ICredentials _credentials;

        public HttpProxy(Uri uri, ICredentials credentials = null) : this(uri.Scheme == "https" ? EnumProxyType.Https : EnumProxyType.Http, credentials)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            Host = uri.Host;
            Port = uri.Port;
            _uri = uri;
        }

        public HttpProxy(string url, ICredentials credentials = null) : this(ObjectCache.CreateUri(url, true), credentials)
        {
        }

        private HttpProxy(EnumProxyType proxyType, ICredentials credentials)
        {
            ProxyType = proxyType;
            _credentials = credentials;
        }

        private HttpProxy() : this(EnumProxyType.None, null)
        {
        }

        public static HttpProxy None { get; set; } = new HttpProxy();

        public Uri GetProxy(Uri destination) => _uri;

        public bool IsBypassed(Uri host) => false;

        public ICredentials Credentials
        {
            get => _credentials;
            set
            {
                if (ProxyType == EnumProxyType.None) throw new NotSupportedException();
                _credentials = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is HttpProxy o && Equals(o);
        }

        public bool Equals(HttpProxy other)
        {
            return other != null
                && _uri == other._uri
                && ProxyType == other.ProxyType
                && Host == other.Host
                && Port == other.Port
                && Credentials == other.Credentials;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_uri != null ? _uri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)ProxyType;
                hashCode = (hashCode * 397) ^ (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Port;
                // hashCode = (hashCode * 397) ^ (Credentials != null ? Credentials.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(HttpProxy left, HttpProxy right)
        {    // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right)) return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(HttpProxy left, HttpProxy right)
        {
            return !(left == right);
        }
    }
}
