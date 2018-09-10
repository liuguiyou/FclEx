using System.Reflection;
using AbpExt.Core.Dependency;
using FclEx.Fw.Dependency.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency.Registration.Conventional
{
    public class GenericConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            Classes.FromAssembly(context.Assembly)
                .BasedOn<IGenericTransientDependency>()
                .If(type => !type.IsAbstract)
                .Register()
                .WithSelf()
                .WithDefaultInterfaces()
                .WithServiceFromInterface(typeof(IGenericTransientDependency), t => t.IsGenericType)
                .Register(context.IocManager, ServiceLifetime.Transient);

            Classes.FromAssembly(context.Assembly)
                .BasedOn<IGenericTransientDependency>()
                .If(type => !type.IsAbstract)
                .Register()
                .WithSelf()
                .WithDefaultInterfaces()
                .WithServiceFromInterface(typeof(IGenericSingletonDependency), t => t.IsGenericType)
                .Register(context.IocManager, ServiceLifetime.Singleton);
        }
    }
}
