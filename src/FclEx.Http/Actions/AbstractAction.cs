using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Event;
using FclEx.Utils;

namespace FclEx.Http.Actions
{
    public abstract class AbstractAction : IAction
    {
        protected string ActionName => GetType().GetDescription();
        protected virtual int MaxReTryTimes { get; set; } = 3;
        protected int ExcuteTimes { get; set; }
        protected int ErrorTimes { get; set; }

        private static Exception DefaultException { get; } = new Exception("Unknown Error");

        protected AbstractAction(ActionEventListener listener = null)
        {
            OnActionEvent += listener;
        }

        protected virtual void LogActionEvent(ActionEvent actionEvent)
        {
            if (!Debugger.IsLogging()) return;

            var type = actionEvent.Type;
            var typeName = type.GetDescription();
            var target = actionEvent.Target;

            switch (type)
            {
                case ActionEventType.EvtError:
                {
                    var ex = (Exception)target;
                    var msg = ex.ToString().TrimEnd();
                    DebuggerHepler.WriteLine($"[Action={ActionName}, Result={typeName}, {msg}]");
                    break;
                }

                case ActionEventType.EvtRetry:
                {
                    var ex = (Exception)target;
                    DebuggerHepler.WriteLine($"[Action={ActionName}, Result={typeName}, ErrorTimes={ErrorTimes}][{ex}]");
                    break;
                }

                case ActionEventType.EvtCanceled:
                    DebuggerHepler.WriteLine($"[Action={ActionName}, Result={typeName}, Target={target}]");
                    break;

                default:
                    DebuggerHepler.WriteLine($"[Action={ActionName}, Result={typeName}]");
                    break;
            }
        }

        protected virtual ValueTask<ActionEvent> NotifyActionEventAsync(ActionEvent actionEvent)
        {
            try
            {
                LogActionEvent(actionEvent);
                return OnActionEvent(this, actionEvent);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(ex);
            }
        }

        protected virtual ValueTask<ActionEvent> HandleExceptionAsync(Exception ex)
        {
            ++ErrorTimes;
            try
            {
                var @event = ActionEvent.Create(ErrorTimes < MaxReTryTimes ?
                    ActionEventType.EvtRetry : ActionEventType.EvtError, ex);
                LogActionEvent(@event);
                return OnActionEvent(this, @event);
            }
            catch (Exception e)
            {
                throw new Exception($"throw an unhandled exception when excute [{nameof(HandleExceptionAsync)}] method.", e);
            }
        }

        protected ValueTask<ActionEvent> NotifyActionEventAsync(ActionEventType type, object target = null)
            => NotifyActionEventAsync(ActionEvent.Create(type, target));

        protected ValueTask<ActionEvent> NotifyOkEventAsync(object target = null)
            => NotifyActionEventAsync(ActionEventType.EvtOk, target);

        protected ValueTask<ActionEvent> NotifyRetryEventAsync(Exception ex) => NotifyActionEventAsync(ActionEventType.EvtRetry, ex);
        protected ValueTask<ActionEvent> NotifyRetryEventAsync(string msg = null) => NotifyActionEventAsync(ActionEventType.EvtRetry, CreateEx(msg));
        protected ValueTask<ActionEvent> NotifyCancelEventAsync() => NotifyActionEventAsync(ActionEventType.EvtCanceled, this);
        protected ValueTask<ActionEvent> NotifyErrorAsync(Exception ex) => NotifyActionEventAsync(ActionEvent.Create(ActionEventType.EvtError, ex));
        protected ValueTask<ActionEvent> NotifyErrorAsync(string msg = null) => NotifyErrorAsync(CreateEx(msg));
        protected ValueTask<ActionEvent> NotifyObjectErrorAsync<T>(T obj, string msg = null, Exception innerException = null)
            => NotifyErrorAsync(ObjectException.Create(obj, msg, innerException));

        private static Exception CreateEx(string msg) => msg.IsNullOrEmpty() ? DefaultException : new Exception(msg);

        protected virtual ValueTask<ActionEvent> ExecuteInternalAsync(CancellationToken token)
        {
            return ActionEvent.EmptyOkEvent.ToValueTask();
        }

        public async ValueTask<ActionEvent> ExecuteAsync(CancellationToken token)
        {
            if (Debugger.IsLogging())
            {
                DebuggerHepler.WriteLine($"[Action={ActionName} Begin]");
                var watch = new Stopwatch();
                watch.Start();
                var result = await ExecuteInternalAsync(token).DonotCapture();
                watch.Stop();
                DebuggerHepler.WriteLine($"[Action={ActionName} End, ResultType={result.Type.GetDescription()}. {watch.ElapsedMilliseconds} ms]");
                return result;
            }
            else
            {
                return await ExecuteInternalAsync(token).DonotCapture();
            }
        }

        public event ActionEventListener OnActionEvent = (sender, @event) => @event.ToValueTask();
    }
}
