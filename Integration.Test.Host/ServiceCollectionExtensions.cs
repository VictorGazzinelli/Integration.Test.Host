using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Test.Host
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceDescriptor GetEntryFor<T>(this IServiceCollection services)
            => services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(T));

        public static ServiceDescriptor GetEntryFor(this IServiceCollection services, Type type)
            => services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == type);
    }
}
