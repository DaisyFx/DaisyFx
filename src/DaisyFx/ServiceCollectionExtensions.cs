using DaisyFx;
using DaisyFx.Events;
using DaisyFx.Hosting;
using DaisyFx.Sources.Http;
using Microsoft.Extensions.Configuration;
using System;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDaisy(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            Action<IDaisyServiceCollection> configureDaisy)
        {
            var hostMode = configuration.GetValue<string>("daisy:mode")?.ToLower() ?? "service";

            configureDaisy(new DaisyServiceCollection(hostMode, serviceCollection, configuration));

            serviceCollection.AddSingleton<HttpChainRouter>();
            serviceCollection.AddSingleton(typeof(EventHandlerCollection<>));
            serviceCollection.AddHostedService(s =>
            {
                if (s.GetService<IHostInterface>() is { } hostInterface)
                    return hostInterface;

                return hostMode switch
                {
                    "console" => ActivatorUtilities.CreateInstance<ConsoleHostInterface>(s),
                    "service" => ActivatorUtilities.CreateInstance<ServiceHostInterface>(s),
                    _ => throw new ArgumentException("mode", $"{hostMode} is not a valid value for daisy:mode")
                };
            });

            return serviceCollection;
        }
    }
}