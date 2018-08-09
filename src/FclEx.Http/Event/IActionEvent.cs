namespace FclEx.Http.Event
{
    public interface IActionEvent
    {
        ActionEventType Type { get; }
        object Target { get; }
    }
}