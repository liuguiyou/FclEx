using System;

namespace FclEx.Http.Event
{
    /// <summary>
    /// 用来表示一个action的执行结果
    /// </summary>
    public struct ActionEvent
    {
        public ActionEventType Type { get; }
        public object Target { get; }

        public static ActionEvent CreateEvent(ActionEventType type, object target) => new ActionEvent(type, target);
        public static ActionEvent CreateOkEvent(object target) => CreateEvent(ActionEventType.EvtOk, target);
        public static ActionEvent CreateErrorEvent(Exception ex) => CreateEvent(ActionEventType.EvtError, ex);
        public static ActionEvent CreateErrorEvent(string msg) => CreateEvent(ActionEventType.EvtError, new Exception(msg));
        public static ActionEvent CreateCancelEvent(object target) => CreateEvent(ActionEventType.EvtCanceled, target);
        public ActionEvent<T> ToExplicit<T>() => new ActionEvent<T>(Type, Target);
        public static implicit operator ActionEvent(Exception ex) => CreateErrorEvent(ex);
        
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


        public static ActionEvent<T> CreateOkEvent<T>(T target) => new ActionEvent<T>(ActionEventType.EvtOk, target);
        public static ActionEvent<T> CreateErrorEvent<T>(Exception ex) => new ActionEvent<T>(ActionEventType.EvtError, ex);
        public static ActionEvent<T> CreateErrorEvent<T>(string msg) => new ActionEvent<T>(ActionEventType.EvtError, new Exception(msg));
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

        public static implicit operator ActionEvent<T>(T item) => ActionEvent.CreateOkEvent(item);
        public static implicit operator ActionEvent(ActionEvent<T> actionEvent) => new ActionEvent(actionEvent.Type, actionEvent.Target);
        public ActionEvent<TTarget> ToExplicit<TTarget>() => new ActionEvent<TTarget>(Type, Target);
        public static implicit operator ActionEvent<T>(Exception ex) => new ActionEvent<T>(ActionEventType.EvtError, ex);
    }
}
