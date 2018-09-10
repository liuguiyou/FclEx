using System;
using System.Collections.Generic;
using System.Linq;

namespace FclEx.Fw.Dependency.Registration
{
    public class RegistrationDescriptor : IRegistration
    {
        private readonly IEnumerable<Type> _types;
        private IEnumerable<(Type T, Type TImpl)> _typePairs;

        public RegistrationDescriptor(IHasTypes hasTypes)
        {
            _types = hasTypes.GetTypes();
            _typePairs = Array.Empty<(Type, Type)>();
        }

        private string GetInterfaceName(Type @interface)
        {
            var name = @interface.Name;
            if ((name.Length > 1 && name[0] == 'I') && char.IsUpper(name[1]))
            {
                return name.Substring(1);
            }
            return name;
        }

        public RegistrationDescriptor WithSelf()
        {
            _typePairs = _typePairs.Concat(_types.Select(m => (m, m)));
            return this;
        }

        public RegistrationDescriptor WithDefaultInterfaces()
        {
            var q = _types.SelectMany(m => m.GetInterfaces().Where(i => m.Name.Contains(GetInterfaceName(i))),
                (c, i) => (i, c));
            _typePairs = _typePairs.Concat(q);
            return this;
        }

        public RegistrationDescriptor WithServiceFromInterface(Type t, Func<Type, bool> filter = null)
        {
            var q = _types.SelectMany(m => m.GetInterfaces()
                    .Where(i => t.IsAssignableFrom(i) && i != t)
                    .WhereIf(filter, filter != null),
                (c, i) => (i, c));
            _typePairs = _typePairs.Concat(q);
            return this;
        }

        public IEnumerable<(Type T, Type TImpl)> GetTypePairs() => _typePairs;
    }
}