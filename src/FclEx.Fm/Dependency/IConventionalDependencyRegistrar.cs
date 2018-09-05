namespace FclEx.Fm.Dependency
{
    public interface IConventionalDependencyRegistrar
    {
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}