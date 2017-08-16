using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx
{
    public class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();

        public void Dispose() { }
    }
}
