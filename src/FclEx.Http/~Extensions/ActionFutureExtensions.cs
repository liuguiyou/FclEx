using System.Collections.Generic;
using FclEx.Http.Actions;

namespace FclEx.Http
{
    public static class ActionFutureExtensions
    {
        public static ActionFuture PushActions(this ActionFuture future, IEnumerable<IAction> actions)
        {
            foreach (var action in actions)
            {
                future.PushAction(action);
            }
            return future;
        }
    }
}
