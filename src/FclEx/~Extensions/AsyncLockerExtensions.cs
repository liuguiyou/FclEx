using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FclEx.Utils;

namespace FclEx
{
    public static class AsyncLockerExtensions
    {
        public static void DoubleCheckAndDo(this AsyncLocker locker, Func<bool> condition, Action action)
        {
            if (condition() && action != null)
            {
                using (locker.Lock())
                {
                    if (condition())
                    {
                        action();
                    }
                }
            }
        }

        public static async Task DoubleCheckAndDoAsync(this AsyncLocker locker, Func<bool> condition, Func<Task> action)
        {
            if (condition() && action != null)
            {
                using (await locker.LockAsync())
                {
                    if (condition())
                    {
                        await action().DonotCapture();
                    }
                }
            }
        }
    }
}
