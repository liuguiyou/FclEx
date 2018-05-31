using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.Event;

namespace FclEx.Http.Actions
{
    public class ActionFuture : IActionFuture
    {
        private readonly ActionEventListener _outerListener;
        // private readonly LinkedList<IAction> _queue = new LinkedList<IAction>();

        private readonly List<Func<object[], IAction>> _queue = new List<Func<object[], IAction>>();

        public ActionFuture(ActionEventListener listener = null)
        {
            _outerListener = listener;
        }

        public virtual async ValueTask<ActionEvent> ExecuteAsync(CancellationToken token)
        {
            var results = new object[_queue.Count];
            var lastEvent = ActionEvent.EmptyOkEvent;
            for (var i = 0; i < _queue.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return ActionEvent.CreateCancelEvent(this);

                var item = _queue[i];
                var action = item(results);
                if (action == null) continue;

                action.OnActionEvent += _outerListener;
                var result = await action.ExecuteAsync(token).ConfigureAwait(false);
                action.OnActionEvent -= _outerListener;

                results[i] = result.Target;
                switch (result.Type)
                {
                    case ActionEventType.EvtError:
                    case ActionEventType.EvtCanceled:
                        return result;

                    case ActionEventType.EvtRetry:
                    case ActionEventType.EvtRepeat:
                        --i; // 回退，即重复执行
                        break;

                    default:
                        lastEvent = result;
                        break;
                }
            }
            return lastEvent;
        }

        public virtual IActionFuture PushAction(IAction action)
        {
            return PushAction(o => action, 0);
        }

        public virtual IActionFuture PushAction(Func<object, IAction> func)
        {
            return PushAction(func, _queue.Count - 1);
        }

        public IActionFuture PushAction(Func<object, IAction> func, int dependentResultIndex)
        {
            _queue.Add(objs => func(objs[dependentResultIndex]));
            return this;
        }

        public IActionFuture PushAction(Func<object[], IAction> func)
        {
            func = func ?? (o => null);
            _queue.Add(func);
            return this;
        }
    }
}
