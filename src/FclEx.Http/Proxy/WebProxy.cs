using System;
using System.Net;

namespace FclEx.Http.Proxy
{
    public sealed class WebProxy : IWebProxy, IEquatable<WebProxy>
    {
        public EnumProxyType ProxyType { get; }
        public string Host { get; }
        public int Port { get; }
        private readonly Uri _uri;
        private ICredentials _credentials;

        public WebProxy(string ip, int port, EnumProxyType proxyType, ICredentials credentials = null) : this(proxyType, credentials)
        {
            Host = ip;
            Port = port;
            if (proxyType == EnumProxyType.Http || proxyType == EnumProxyType.Https)
            {
                var scheme = proxyType == EnumProxyType.Http ? "http" : "https";
                var url = $"{scheme}://{ip}";
                if (!port.IsDefault()) url = $"{url}:{port}";
                _uri = new Uri(url);
            }
        }

        public WebProxy(Uri uri, ICredentials credentials = null) : this(uri.Scheme == "https" ? EnumProxyType.Https : EnumProxyType.Http, credentials)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            Host = uri.Host;
            Port = uri.Port;
            _uri = uri;
        }

        public WebProxy(string url, ICredentials credentials = null) : this(ObjectCache.CreateUri(url, true), credentials)
        {
        }

        private WebProxy(EnumProxyType proxyType, ICredentials credentials)
        {
            ProxyType = proxyType;
            _credentials = credentials;
        }

        private WebProxy() : this(EnumProxyType.None, null)
        {
        }

        public static WebProxy None { get; set; } = new WebProxy();

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
            return obj is WebProxy o && Equals(o);
        }

        public bool Equals(WebProxy other)
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

        public static bool operator ==(WebProxy left, WebProxy right)
        {    // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right)) return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(WebProxy left, WebProxy right)
        {
            return !(left == right);
        }
    }
}
