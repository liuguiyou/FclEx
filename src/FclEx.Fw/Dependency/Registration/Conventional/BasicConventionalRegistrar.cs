using System.Reflection;
using FclEx.Fw.Dependency.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency.Registration.Conventional
{
    /// <summary>
    /// This class is used to register basic dependency implementations such as <see cref="ITransientDependency"/> and <see cref="ISingletonDependency"/>.
    /// </summary>
    public class BasicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            Classes.FromAssembly(context.Assembly)
                .BasedOn<ITransientDependency>()
                .If(type => !type.IsAbstract)
                .Register()
                .WithSelf()
                .WithDefaultInterfaces()
                .Register(context.IocManager, ServiceLifetime.Transient);

            Classes.FromAssembly(context.Assembly)
                .BasedOn<ISingletonDependency>()
                .If(type => !type.IsAbstract)
                .Register()
                .WithSelf()
                .WithDefaultInterfaces()
                .Register(context.IocManager, ServiceLifetime.Singleton);
        }
    }
}