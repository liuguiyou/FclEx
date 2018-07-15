using System.Collections.Generic;

namespace FclEx.Http.Proxy
{
    public class ProxyFinderSettings
    {
        private readonly HashSet<ProxyType> _proxyTypes = new HashSet<ProxyType>();

        public ProxyFinderSettings(string globalProxy, IEnumerable<ProxyType> proxyTypes)
        {
            GlobalProxy = globalProxy;
            _proxyTypes.AddRangeSafely(proxyTypes);
        }

        public ProxyFinderSettings(string globalProxy, ProxyType proxyType) : this(globalProxy, new[] { proxyType })
        {
        }

        public string GlobalProxy { get; }

        public IReadOnlyCollection<ProxyType> ProxyTypes => _proxyTypes;

        public static ProxyFinderSettings Default { get; } = new ProxyFinderSettings(null, ProxyType.Http);
    }
}
