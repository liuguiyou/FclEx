using System;
using System.Text;

namespace FclEx.Utils
{
    public class ObjectException
    {
        public static ObjectException<T> Create<T>(T obj, string msg = null, Exception inner = null)
            => new ObjectException<T>(obj, msg, inner);
    }

    public class ObjectException<T> : SimpleException
    {
        public T Target { get; set; }

        public ObjectException(T obj, string msg = null, Exception inner = null)
            : base(msg, inner)
        {
            Target = obj;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(GetType().ShortName(), 256);
            sb.AppendIf(() => ": " + Message, !Message.IsNullOrEmpty());
            sb.AppendLineIf(() => " ---> " + InnerException, InnerException != null);
            sb.AppendIfNotEmpty(StackTrace);
            return sb.ToString();
        }
    }
}
