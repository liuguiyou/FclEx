namespace FclEx.Http.Actions
{
    public class ActionFactory : IActionFactory
    {
        public virtual IAction CreateAction<T>(params object[] parameters) where T : IAction
        {
            return (IAction)typeof(T).CreateObject(parameters);
        }
    }
}
