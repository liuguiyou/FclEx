using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
using AspectCore.Injector;
using LightInject;
using IServiceContainer = LightInject.IServiceContainer;

namespace AspectCore.Extensions.LightInject
{
    public enum RegistryType
    {
        ByType,
        ByInstance,
        ByFactory
    }

    public static class ContainerBuilderExtensions
    {
        private static readonly string[] _nonAspect =
        {
            "LightInject.*",
            "LightInject"
        };

        private static readonly string[] _excepts = new[]
        {
            "Microsoft.Extensions.Logging",
            "Microsoft.Extensions.Options",
            "System",
            "System.*",
            "IHttpContextAccessor",
            "ITelemetryInitializer",
            "IHostingEnvironment",
        }.Concat(_nonAspect).ToArray();

        public static IServiceContainer RegisterDynamicProxy(this IServiceContainer container,
            IAspectConfiguration aspectConfig = null,
            Action<IAspectConfiguration> configure = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            aspectConfig = aspectConfig ?? new AspectConfiguration();

            foreach (var m in _nonAspect)
            {
                aspectConfig.NonAspectPredicates.AddNamespace(m);
            }

            configure?.Invoke(aspectConfig);

            container.AddSingleton<IAspectConfiguration>(aspectConfig)
                .AddTransient(typeof(IManyEnumerable<>), typeof(ManyEnumerable<>))
                .AddSingleton<IServiceContainer>(container)
                .AddSingleton<IServiceProvider, LightInjectServiceResolver>()
                .AddSingleton<IServiceResolver, LightInjectServiceResolver>()
                .AddSingleton<IAspectContextFactory, AspectContextFactory>()
                .AddSingleton<IAspectActivatorFactory, AspectActivatorFactory>()
                .AddSingleton<IProxyGenerator, ProxyGenerator>()
                .AddSingleton<IParameterInterceptorSelector, ParameterInterceptorSelector>()
                .AddSingleton<IPropertyInjectorFactory, PropertyInjectorFactory>()
                .AddSingleton<IInterceptorCollector, InterceptorCollector>()
                .AddSingleton<IInterceptorSelector, ConfigureInterceptorSelector>()
                .AddSingleton<IInterceptorSelector, AttributeInterceptorSelector>()
                .AddSingleton<IAdditionalInterceptorSelector, AttributeAdditionalInterceptorSelector>()
                .AddSingleton<IAspectValidatorBuilder, AspectValidatorBuilder>()
                .AddSingleton<IAspectBuilderFactory, AspectBuilderFactory>()
                .AddSingleton<IProxyTypeGenerator, ProxyTypeGenerator>()
                .AddSingleton<IAspectCachingProvider, AspectCachingProvider>();

            container.Decorate(aspectConfig.CreateDecorator());

            return container;
        }

        private static RegistryType GetRegistryType(this ServiceRegistration registration)
        {
            if (registration.FactoryExpression != null) return RegistryType.ByFactory;
            else if (registration.Value != null) return RegistryType.ByInstance;
            else return RegistryType.ByType;
        }

        private static Type GetImplType(this ServiceRegistration registration)
        {
            switch (registration.GetRegistryType())
            {
                case RegistryType.ByType: return registration.ImplementingType;
                case RegistryType.ByInstance: return registration.Value.GetType();
                case RegistryType.ByFactory: return registration.FactoryExpression.Method.ReturnType;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        private static DecoratorRegistration CreateDecorator(this IAspectConfiguration aspectConfiguration)
        {
            var reg = new DecoratorRegistration()
            {
                CanDecorate = s => CanDecorate(s, aspectConfiguration),
                ImplementingTypeFactory = CreateProxyType
            };
            return reg;
        }

        private static Type CreateProxyType(IServiceFactory factory, ServiceRegistration registration)
        {
            var serviceType = registration.ServiceType.GetTypeInfo();
            var implType = registration.GetImplType();
            var proxyTypeGenerator = factory.GetInstance<IProxyTypeGenerator>();

            if (serviceType.IsClass)
            {
                return proxyTypeGenerator.CreateClassProxyType(serviceType, implType);
            }
            else if (serviceType.IsGenericTypeDefinition)
            {
                return proxyTypeGenerator.CreateClassProxyType(implType, implType);
            }
            else
            {
                return proxyTypeGenerator.CreateInterfaceProxyType(serviceType, implType);
            }
        }

        private static bool CanDecorate(ServiceRegistration registration, IAspectConfiguration aspectConfiguration)
        {
            var serviceType = registration.ServiceType.GetTypeInfo();
            var implType = registration.GetImplType().GetTypeInfo();

            if (implType.IsProxy() || !implType.CanInherited())
            {
                return false;
            }
            if (_excepts.Any(x => implType.Name.Matches(x)) || _excepts.Any(x => implType.Namespace.Matches(x)))
            {
                return false;
            }
            if (!serviceType.CanInherited() || serviceType.IsNonAspect())
            {
                return false;
            }

            var aspectValidator = new AspectValidatorBuilder(aspectConfiguration).Build();
            if (!aspectValidator.Validate(serviceType, true) && !aspectValidator.Validate(implType, false))
            {
                return false;
            }
            return true;
        }

    }
}
