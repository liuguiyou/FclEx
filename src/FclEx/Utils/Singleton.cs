using System;

namespace FclEx.Utils
{
    public static class Singleton<T> where T : class
    {
        private static T _instance;
        private static readonly object _lock = new object();
        
        public static T GetOrInit(Func<T> func)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = func();
                    }
                }
            }
            return _instance;
        }
    }
}
