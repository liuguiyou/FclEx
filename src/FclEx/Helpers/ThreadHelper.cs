using System.Threading;

namespace FclEx.Helpers
{
    public static class ThreadHelper
    {
        public static void Sleep(int seconds)
        {
            if (seconds > 0)
                Thread.Sleep(seconds * 1000);
        }
    }
}
