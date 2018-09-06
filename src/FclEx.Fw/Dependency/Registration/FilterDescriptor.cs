using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FclEx.Fw.Dependency.Registration
{
    public class FilterDescriptor : IHasTypes
    {
        private readonly IEnumerable<Type> _types;
        private readonly Func<Type, bool> _filter;

        public FilterDescriptor(IHasTypes hasTypes, Func<Type, bool> filter)
        {
            _filter = filter;
            _types = hasTypes.GetTypes();
        }

        public IEnumerable<Type> GetTypes()
        {
            return _types.Where(_filter);
        }
    }
}
