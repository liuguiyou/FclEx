using System;
using System.Threading;

namespace FclEx.Utils
{
    public struct Timer<T> : IDisposable
    {
        private Timer _timer;

        public Timer(TimerCallback<T> callback, T state, TimeSpan dueTime, TimeSpan period)
        {
            Check.NotNull(callback, nameof(callback));
            _timer = new Timer(s => callback(s.CastTo<T>()), state, dueTime, period);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;
        }

        public bool Available => _timer != null;
    }
}
