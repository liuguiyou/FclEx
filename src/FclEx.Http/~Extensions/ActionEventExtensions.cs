using FclEx.Http.Event;

namespace FclEx.Http
{
    public static class ActionEventExtensions
    {
        public static T Get<T>(this ActionEvent e)
        {
            return (T)e.Target;
        }

        public static bool TryGet<T>(this ActionEvent e, out T result)
        {
            if (e.Target is T r)
            {
                result = r;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public static T GetOrDefault<T>(this ActionEvent e, T defaultValue = default(T))
        {
            return e.Target is T variable ? variable : defaultValue;
        }
    }
}
