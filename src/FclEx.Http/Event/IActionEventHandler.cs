namespace FclEx.Http.Event
{
    public interface IActionEventHandler
    {
        event ActionEventListener OnActionEvent;
    }
}
