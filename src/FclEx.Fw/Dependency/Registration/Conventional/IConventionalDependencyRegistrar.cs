namespace FclEx.Fw.Dependency.Registration.Conventional
{
    public interface IConventionalDependencyRegistrar
    {
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}