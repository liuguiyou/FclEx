using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FclEx.Utils
{
    public class TimerLazy<T>
    {
        private readonly object _lock = new object();
        private volatile Lazy<T> _lazy;
        private readonly Timer _timer;

        public TimerLazy(Func<T> valueFactory, LazyThreadSafetyMode mode, TimeSpan span)
        {
            _lazy = new Lazy<T>(valueFactory, mode);
            _timer = new Timer(o =>
             {
                 if (_lazy.IsValueCreated)
                 {
                     lock (_lock)
                     {
                         if (_lazy.IsValueCreated)
                         {
                             if (_lazy.Value is IDisposable disposable)
                                 disposable.Dispose();
                             _lazy = new Lazy<T>(valueFactory, mode);
                         }
                     }
                 }
             }, null, span, span);
        }

        public T Value => _lazy.Value;
    }
}
