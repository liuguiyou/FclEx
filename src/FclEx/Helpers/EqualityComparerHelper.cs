using System;
using System.Collections.Generic;

namespace FclEx.Helpers
{
    public static class EqualityComparerHelper
    {
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> compareFunc, Func<T, int> hashFunc)
        {
            return new CommonComparer<T>(compareFunc, hashFunc);
        }

        private class CommonComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _compareFunc;
            private readonly Func<T, int> _hashFunc;

            public CommonComparer(Func<T, T, bool> compareFunc, Func<T, int> hashFunc)
            {
                _compareFunc = compareFunc ?? throw new ArgumentNullException(nameof(compareFunc));
                _hashFunc = hashFunc ?? (x => x.GetHashCode());
            }

            public bool Equals(T x, T y)
            {
                return _compareFunc(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _hashFunc(obj);
            }
        }

        public static IEqualityComparer<T> Create<T, TKey>(Func<T, TKey> keySelector, 
            IEqualityComparer<TKey> comparer = null)
        {
            return new KeyComparer<T, TKey>(keySelector, comparer);
        }

        private class KeyComparer<T, TKey> : IEqualityComparer<T>
        {
            private readonly Func<T, TKey> _keySelector;
            private readonly IEqualityComparer<TKey> _comparer;

            public KeyComparer(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            {
                _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
                _comparer = comparer ?? EqualityComparer<TKey>.Default;
            }

            public bool Equals(T x, T y)
            {
                return _comparer.Equals(_keySelector(x), _keySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return _comparer.GetHashCode(_keySelector(obj));
            }
        }
    }
}
