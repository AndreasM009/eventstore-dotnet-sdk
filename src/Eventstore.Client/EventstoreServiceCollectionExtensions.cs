using System;
using Microsoft.Extensions.DependencyInjection;

namespace Eventstore.Client
{
    public static class EventstoreServiceCollectionExtensiosn
    {
        public static IServiceCollection AddEventstoreClient(this IServiceCollection services, Action<EventstoreClientBuilder> configure = null)
        {
            if (null == services)
                throw new ArgumentNullException(nameof(services));

            if (services.Contains(ServiceDescriptor.Singleton<EventstoreServiceMarker, EventstoreServiceMarker>()))
                return services;

            services.AddSingleton<EventstoreServiceMarker>();

            services.AddSingleton(_ => 
            {
                var builder = new EventstoreClientBuilder();

                configure?.Invoke(builder);

                return builder.Build();
            });
            return services;
        }

        private class EventstoreServiceMarker 
        {

        }
    }
}