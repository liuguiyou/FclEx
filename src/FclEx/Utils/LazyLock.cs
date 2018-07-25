using System;

namespace FclEx.Utils
{
    public sealed class LazyLock<T>
    {
        private volatile bool _got;
        private T _value;

        public T Get(Func<T> activator = null)
        {
            if (!_got)
            {
                lock (this)
                {
                    if (!_got && activator != null)
                    {
                        _value = activator();
                        _got = true;
                    }
                }
            }
            return _value;
        }
    }
}
