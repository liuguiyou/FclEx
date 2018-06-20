using System;
using FclEx.Http.Event;

namespace FclEx.Http
{
    public static class ActionEventExtensions
    {
        public static bool True<T>(this ActionEvent @event, Func<T, bool> predicate)
        {
            return @event.TryGet<T>(out var result) && predicate(result);
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

        public static bool TrueFromObjEx<T>(this ActionEvent @event, Func<T, bool> predicate)
        {
            return @event.TryGetFromObjEx<T>(out var result) && predicate(result);
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
    }
}
