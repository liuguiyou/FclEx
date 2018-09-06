using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Fw.Dependency.Registration
{
    public interface IRegistration
    {
        IEnumerable<(Type T, Type TImpl)> GetTypePairs();
    }
}
