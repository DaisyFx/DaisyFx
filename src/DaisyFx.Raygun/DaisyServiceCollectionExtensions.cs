using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mindscape.Raygun4Net;

namespace DaisyFx.Raygun
{
    public static class DaisyServiceCollectionExtensions
    {
        public static IDaisyServiceCollection AddRaygun(this IDaisyServiceCollection daisy)
        {
            return daisy.AddRaygun<RaygunExceptionEventHandler>();
        }

        public static IDaisyServiceCollection AddRaygun<THandler>(this IDaisyServiceCollection daisy)
            where THandler : RaygunExceptionEventHandler
        {
            ThrowIfAlreadyAdded(daisy.ServiceCollection);

            daisy.ServiceCollection.Configure<RaygunSettings>(daisy.Configuration.GetSection("RaygunSettings"));
            daisy.ServiceCollection.TryAddSingleton<IRaygunClientProvider, RaygunClientProvider>();

            daisy.AddEventHandlerSingleton<THandler>();

            return daisy;
        }

        private static void ThrowIfAlreadyAdded(IServiceCollection serviceCollection)
        {
            var alreadyAdded = serviceCollection.Any(s => s.ServiceType == typeof(RaygunMarkerService));
            if (alreadyAdded)
            {
                throw new InvalidOperationException($"{nameof(AddRaygun)} was called multiple times");
            }

            serviceCollection.TryAddSingleton<RaygunMarkerService>();
        }
    }
}