using System;
using System.Collections.Generic;
using FclEx.Http.Actions;
using FclEx.Utils;

namespace FclEx.Http
{
    public static class ActionFutureExtensions
    {
        public static IActionFuture PushAction(this IActionFuture future, IAction action)
        {
            Check.NotNull(action, nameof(action));
            return future.PushAction(objs => action);
        }

        public static IActionFuture PushAction(this IActionFuture future,
            Func<object, IAction> func, int dependentResultIndex)
        {
            Check.NotNull(func, nameof(func));
            return future.PushAction(objs => func(objs[dependentResultIndex]));
        }

        public static IActionFuture PushAction<TResult>(this IActionFuture future,
            Func<TResult, IAction> func, int dependentResultIndex)
        {
            Check.NotNull(func, nameof(func));
            return future.PushAction(objs => func(objs[dependentResultIndex].CastTo<TResult>()));
        }

        public static IActionFuture PushAction(this IActionFuture future, Func<object, IAction> func)
        {
            Check.NotNull(func, nameof(func));
            return PushAction(future, func, future.Count - 1);
        }

        public static IActionFuture PushAction<TResult>(this IActionFuture future, Func<TResult, IAction> func)
        {
            Check.NotNull(func, nameof(func));
            return PushAction<TResult>(future, func, future.Count - 1);
        }

        public static IActionFuture PushActions(this IActionFuture future, IEnumerable<IAction> actions)
        {
            foreach (var action in actions)
            {
                PushAction(future, action);
            }
            return future;
        }
    }
}
