using System;
using System.Collections.Generic;
using System.Text;
using static System.Environment;

namespace FclEx.Utils
{
    public class SimpleException : Exception
    {
        public SimpleException(string msg) : this(msg, Environment.StackTrace)
        {
        }

        public SimpleException(string msg, Exception inner) : this(msg, Environment.StackTrace, inner)
        {
        }


        public SimpleException(string msg, string stackTrace) : base(msg)
        {
            StackTrace = stackTrace ?? Environment.StackTrace;
        }

        public SimpleException(string msg, string stackTrace, Exception inner) : base(msg, inner)
        {
            StackTrace = stackTrace ?? Environment.StackTrace;
        }

        public override string StackTrace { get; }

        public override string ToString()
        {
            var sb = new StringBuilder(GetType().ShortName(), 256);
            sb.AppendIf(() => ": " + Message, !Message.IsNullOrEmpty());
            sb.AppendIf(() => " ---> " + InnerException, InnerException != null);
            sb.AppendIf(NewLine + StackTrace, !StackTrace.IsNullOrEmpty());
            return sb.ToString();
        }
    }
}
