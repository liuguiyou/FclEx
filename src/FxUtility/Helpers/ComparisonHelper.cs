using System;
using System.Collections.Generic;

namespace FxUtility.Helpers
{
    public static class ComparisonHelper<T>
    {
        public static IComparer<T> CreateComparer<V>(Func<T, V> keySelector, IComparer<V> comparer = null)
        {
            return new CommonComparer<V>(keySelector, comparer);
        }

        class CommonComparer<V> : IComparer<T>
        {
            private readonly Func<T, V> _keySelector;
            private readonly IComparer<V> _comparer;

            public CommonComparer(Func<T, V> keySelector, IComparer<V> comparer = null)
            {
                _keySelector = keySelector;
                _comparer = comparer ?? Comparer<V>.Default;
            }

            public int Compare(T x, T y)
            {
                return _comparer.Compare(_keySelector(x), _keySelector(y));
            }
        }
    }
}
