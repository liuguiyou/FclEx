namespace FclEx.Http.Actions
{
    public interface IActionFactory
    {
        IAction CreateAction<T>(params object[] parameters) where T : IAction;
    }
}
