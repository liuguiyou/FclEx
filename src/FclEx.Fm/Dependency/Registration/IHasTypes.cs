using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Fm.Dependency.Registration
{
    public interface IHasTypes
    {
        IEnumerable<Type> GetTypes();
    }
}
