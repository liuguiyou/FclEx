using FclEx.Http.Event;

namespace FclEx.Http.Actions
{  
    public interface IAction : IActor, IActionEventHandler
    {
    }
}
