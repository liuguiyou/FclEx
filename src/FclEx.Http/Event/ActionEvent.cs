using System;
using System.Threading.Tasks;

namespace FclEx.Http.Event
{
    /// <summary>
    /// 用来表示一个action的执行结果
    /// </summary>
    public struct ActionEvent
    {
        public ActionEventType Type { get; }
        public object Target { get; }

        public static ActionEvent Repeat() => new ActionEvent(ActionEventType.EvtRepeat, null);
        public static ActionEvent Create(ActionEventType type, object target) => new ActionEvent(type, target);
        public static ActionEvent Ok(object target) => Create(ActionEventType.EvtOk, target);
        public static ActionEvent Error(Exception ex) => Create(ActionEventType.EvtError, ex);
        public static ActionEvent Error(string msg) => Create(ActionEventType.EvtError, new Exception(msg));
        public static ActionEvent Cancel(object target) => Create(ActionEventType.EvtCanceled, target);
        public static ActionEvent<T> Ok<T>(T target) => new ActionEvent<T>(ActionEventType.EvtOk, target);
        public static ActionEvent<T> Error<T>(Exception ex) => new ActionEvent<T>(ActionEventType.EvtError, ex);
        public static ActionEvent<T> Error<T>(string msg) => new ActionEvent<T>(ActionEventType.EvtError, new Exception(msg));
        public ActionEvent<T> ToExplicit<T>() => new ActionEvent<T>(Type, Target);
        public static implicit operator ActionEvent(Exception ex) => Error(ex);
        public static implicit operator Task<ActionEvent>(ActionEvent actionEvent) => actionEvent.ToTask();
        public static implicit operator ValueTask<ActionEvent>(ActionEvent actionEvent) => actionEvent.ToValueTask();

        public override string ToString() => $"{Type.GetFullDescription()}, target={Target ?? ""}]";

        public ActionEvent(ActionEventType type, object target)
        {
            Type = type;
            Target = target;
        }

        public static ActionEvent EmptyOkEvent { get; } = new ActionEvent(ActionEventType.EvtOk, null);


        public Exception Exception => IsError ? (Exception)Target : null;
        public string ExceptionMessage => Exception?.Message;
        public bool IsOk => Type == ActionEventType.EvtOk;
        public bool IsError => Type == ActionEventType.EvtError;
        public bool IsRetry => Type == ActionEventType.EvtRetry;
    }

    public struct ActionEvent<T>
    {
        public T Result => IsOk ? (T)Target : default;
        public Exception Exception => IsError ? (Exception)Target : null;
        public string ExceptionMessage => Exception?.Message;
        public bool IsOk => Type == ActionEventType.EvtOk;
        public bool IsError => Type == ActionEventType.EvtError;
        public bool IsRetry => Type == ActionEventType.EvtRetry;

        public ActionEvent(ActionEventType type, object target)
        {
            Type = type;
            Target = target;
        }

        public ActionEventType Type { get; }
        public object Target { get; }

        public static implicit operator ActionEvent<T>(T item) => ActionEvent.Ok(item);
        public static implicit operator ActionEvent(ActionEvent<T> actionEvent) => new ActionEvent(actionEvent.Type, actionEvent.Target);
        public ActionEvent<TTarget> ToExplicit<TTarget>() => new ActionEvent<TTarget>(Type, Target);
        public static implicit operator ActionEvent<T>(Exception ex) => new ActionEvent<T>(ActionEventType.EvtError, ex);
        public static implicit operator Task<ActionEvent<T>>(ActionEvent<T> actionEvent) => actionEvent.ToTask();
        public static implicit operator ValueTask<ActionEvent<T>>(ActionEvent<T> actionEvent) => actionEvent.ToValueTask();
    }
}
