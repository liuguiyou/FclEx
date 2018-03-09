using System;
using System.Runtime.ExceptionServices;

namespace FclEx
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception ex)
        {
            return ex.InnerException != null ? $"{ex.Message}[{GetAllMessages(ex.InnerException)}]" : ex.Message;
        }

        public static void ReThrow(this Exception ex) => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
