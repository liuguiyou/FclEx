using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Fm.Dependency.Registration
{
    public interface IRegistration
    {
        IEnumerable<(Type T, Type TImpl)> GetTypePairs();
    }
}
