using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FclEx.Http.Event
{
    [JsonConverter(typeof(ActionEventJsonConverter))]
    public struct ActionEvent<T> : IActionEvent
    {
        [JsonIgnore] public Exception Exception => IsError ? (Exception)Target : null;
        [JsonIgnore] public string ExMsg => Exception?.Message;
        [JsonIgnore] public bool IsOk => Type == ActionEventType.EvtOk;
        [JsonIgnore] public bool IsError => Type == ActionEventType.EvtError;
        [JsonIgnore] public bool IsRetry => Type == ActionEventType.EvtRetry;
        [JsonIgnore] public T Result => IsOk ? (T)Target : default;

        public ActionEvent(ActionEventType type, object target)
        {
            Type = type;
            Target = target;
        }

        public ActionEvent(ActionEventType type, T target)
            : this(type, (object)target)
        {
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
