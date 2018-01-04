using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx
{
    public class AutoMapperIgnoreAttribute : Attribute
    {
        public Type SourceType { get; set; }
    }
}
