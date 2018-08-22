using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
