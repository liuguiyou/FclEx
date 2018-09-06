using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Fw.Dependency;
using FclEx.Fw.Dependency.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace FclEx.Fw.Extensions
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

        public static IRegistration Register(this IRegistration registration, IIocRegistrar registrar,
            ServiceLifetime lifetime)
        {
            foreach (var pair in registration.GetTypePairs())
            {
                registrar.Register(pair.T, pair.TImpl, lifetime);
            }
            return registration;
        }
    }
}
