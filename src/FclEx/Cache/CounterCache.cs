using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using FclEx.Utils;
using MoreLinq;

namespace FclEx.Cache
{
    /// <summary>
    /// A very simple memory-cache which has the capacity.
    /// <para>If it is full before add new item, the minimum usage item will be removed.</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerTypeProxy(typeof(TinyCacheDebugView<,>))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class CounterCache<TKey, TValue>
    {
        private readonly IDictionary<TKey, Counter> _dic;
        private readonly ReaderWriterLockSlim _lock;
        private readonly int? _capacity;

        public CounterCache(int? capacity = null, IEqualityComparer<TKey> comparer = null)
        {
            if (capacity > 0)
                _capacity = capacity;

            comparer = comparer ?? EqualityComparer<TKey>.Default;
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _dic = new Dictionary<TKey, Counter>(comparer);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> activator)
        {
            Counter counter;
            bool success;

            _lock.EnterReadLock();

            try
            {
                success = _dic.TryGetValue(key, out counter);
                if (success) counter.Incre();
            }
            finally
            {
                _lock.ExitReadLock();
            }

            if (!success)
            {
                _lock.EnterWriteLock();
                try
                {
                    if (_capacity.HasValue)
                    {
                        if (_dic.Count >= _capacity)
                        {
                            var min = _dic.MinBy(m => m.Value.Count).OrderBy(m => m.Key).First();
                            _dic.Remove(min);
                        }
                    }
                    counter = new Counter();
                    _dic[key] = counter;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            return counter.Item.Get(() => activator(key));
        }

        public int Count => Read(() => _dic.Count);

        public void Clear()
        {
            _lock.EnterWriteLock();

            try
            {
                _dic.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public TKey[] GetAllKeys() => Read(() => _dic.Keys.ToArray());

        public bool IsFull() => Read(() => _dic.Count >= _capacity);

        internal class Counter
        {
            public void Incre() => ++Count;
            public int Count { get; private set; } = 0;
            public LazyLock<TValue> Item { get; } = new LazyLock<TValue>();
        }

        private T Read<T>(Func<T> func)
        {
            _lock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    internal sealed class TinyCacheDebugView<TKey, TValue>
    {
        private readonly CounterCache<TKey, TValue> _dic;

        public TinyCacheDebugView(CounterCache<TKey, TValue> dictionary)
        {
            _dic = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TKey[] Items => _dic.GetAllKeys();
    }
}
