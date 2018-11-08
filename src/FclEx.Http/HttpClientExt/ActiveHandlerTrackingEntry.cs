using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using FclEx.Utils;

namespace FclEx.Http.HttpClientExt
{
    // Thread-safety: We treat this class as immutable except for the timer. Creating a new object
    // for the 'expiry' pool simplifies the threading requirements significantly.
    internal class ActiveHandlerTrackingEntry
    {
        private static readonly TimerCallback<ActiveHandlerTrackingEntry> _timerCallback = s => s.Timer_Tick();
        private readonly object _lock;
        private bool _timerInitialized;
        private Timer<ActiveHandlerTrackingEntry> _timer;
        private TimerCallback<ActiveHandlerTrackingEntry> _callback;

        public ActiveHandlerTrackingEntry(
            string name,
            LifetimeTrackingHttpMessageHandler handler,
            TimeSpan lifetime)
        {
            Name = name;
            Handler = handler;
            Lifetime = lifetime;
            _lock = new object();
        }

        public LifetimeTrackingHttpMessageHandler Handler { get; private set; }

        public TimeSpan Lifetime { get; }
        public string Name { get; }

        public void StartExpiryTimer(TimerCallback<ActiveHandlerTrackingEntry> callback)
        {
            if (Lifetime == Timeout.InfiniteTimeSpan)
                return; // never expires.

            if (Volatile.Read(ref _timerInitialized))
                return;

            StartExpiryTimerSlow(callback);
        }

        private void StartExpiryTimerSlow(TimerCallback<ActiveHandlerTrackingEntry> callback)
        {
            Debug.Assert(Lifetime != Timeout.InfiniteTimeSpan);

            lock (_lock)
            {
                if (Volatile.Read(ref _timerInitialized))
                    return;

                _callback = callback;
                _timer = NonCapturingTimer.Create(_timerCallback, this, Lifetime, Timeout.InfiniteTimeSpan);
                Volatile.Write(ref _timerInitialized, true);
            }
        }

        private void Timer_Tick()
        {
            Debug.Assert(_callback != null);
            Debug.Assert(_timer.Available);

            lock (_lock)
            {
                _timer.Dispose();
                _callback(this);
            }
        }
    }
}
