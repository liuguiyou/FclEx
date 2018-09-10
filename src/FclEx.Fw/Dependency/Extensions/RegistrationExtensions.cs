using System;
using FclEx.Fw.Dependency.Registration;
using LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Dependency.Extensions
{
    public static class RegistrationExtensions
    {
        public static RegistrationDescriptor Register(this IHasTypes types)
        {
            return new RegistrationDescriptor(types);
        }

        public static IHasTypes If(this IHasTypes types, Func<Type, bool> filter)
        {
            return new FilterDescriptor(types, filter);
        }

        public static IHasTypes BasedOn<T>(this IHasTypes types)
        {
            return new BasedOnDescriptor(types, typeof(T));
        }

        public static IRegistration Register(this IRegistration registration, 
            IServiceContainer services,
            ServiceLifetime lifetime)
        {
            foreach (var pair in registration.GetTypePairs())
            {
                services.Register(pair.T, pair.TImpl, lifetime.ToLightInjectLifetime());
            }
            return registration;
        }

        public static IRegistration Register(this IRegistration registration,
            IIocManager iocManager,
            ServiceLifetime lifetime)
        {
           
            return Register(registration, iocManager.Container, lifetime);
        }
    }
}
