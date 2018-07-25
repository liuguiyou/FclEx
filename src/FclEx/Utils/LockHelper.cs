using System;

namespace WeiboPoster.Utils
{
    public static class LockHelper
    {
        public static void DoubleCheckAndDo(Func<bool> condition, object lockObj, Action action)
        {
            if (condition() && action != null)
            {
                lock (lockObj)
                {
                    if (condition())
                    {
                        action();
                    }
                }
            }
        }
    }
}
