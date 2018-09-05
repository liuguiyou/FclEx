﻿using System;
using System.Linq;
using System.Reflection;
using FclEx.Fm.Dependency.Registration;
using FclEx.Fm.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq.Extensions;

namespace FclEx.Fm.Dependency
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
                .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                .Register()
                .WithSelf()
                .WithDefaultInterfaces()
                .Register(context.IocManager, ServiceLifetime.Transient)
                .Register(context.IocManager, ServiceLifetime.Singleton);
        }
    }
}