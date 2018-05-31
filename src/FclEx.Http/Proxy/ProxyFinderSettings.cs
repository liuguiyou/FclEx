using System.Collections.Generic;

namespace FclEx.Http.Proxy
{
    public class ProxyFinderSettings
    {
        private readonly HashSet<EnumProxyType> _proxyTypes = new HashSet<EnumProxyType>();

        public ProxyFinderSettings(string globalProxy, IEnumerable<EnumProxyType> proxyTypes)
        {
            GlobalProxy = globalProxy;
            _proxyTypes.AddRangeSafely(proxyTypes);
        }

        public ProxyFinderSettings(string globalProxy, EnumProxyType proxyType) : this(globalProxy, new[] { proxyType })
        {
        }

        public string GlobalProxy { get; }

        public IReadOnlyCollection<EnumProxyType> ProxyTypes => _proxyTypes;

        public static ProxyFinderSettings Default { get; } = new ProxyFinderSettings(null, EnumProxyType.Http);
    }
}
