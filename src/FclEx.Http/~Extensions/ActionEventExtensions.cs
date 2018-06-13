using System;
using FclEx.Http.Event;

namespace FclEx.Http
{
    public static class ActionEventExtensions
    {
        public static T Get<T>(this ActionEvent e)
        {
            return (T)e.Target;
        }

        public static bool TryGetFromObjEx<T>(this ActionEvent e, out T result)
        {
            if (e.IsError && e.Target is ObjectException<T> ex)
            {
                result = ex.Target;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static bool TryGet<T>(this ActionEvent e, out T result)
        {
            if (e.IsOk && e.Target is T r)
            {
                result = r;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static T GetOrDefault<T>(this ActionEvent e, T defaultValue = default)
        {
            return e.Target is T variable ? variable : defaultValue;
        }
    }
}
