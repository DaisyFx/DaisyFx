using System;
using System.Collections.Generic;
using System.Linq;
using DaisyFx.Events;
using DaisyFx.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DaisyFx
{
    public class DaisyServiceCollection : IDaisyServiceCollection
    {
        private readonly string _hostMode;
        private readonly HashSet<string> _registeredModes = new();
        private readonly HashSet<object> _registeredChains = new();
        private readonly IServiceCollection _serviceCollection;
        private readonly IConfiguration _configuration;

        public DaisyServiceCollection(string hostMode, IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            _hostMode = hostMode;
            _serviceCollection = serviceCollection;
            _configuration = configuration;
        }

        IServiceCollection IDaisyServiceCollection.ServiceCollection => _serviceCollection;
        IConfiguration IDaisyServiceCollection.Configuration => _configuration;

        public DaisyServiceCollection AddHostMode<THostInterface>(string alias,
            Action<DaisyServiceCollection, IServiceCollection, IConfiguration> configureServices)
            where THostInterface : class, IHostInterface
        {
            if (!_registeredModes.Add(alias))
            {
                throw new NotSupportedException($"{nameof(alias)}: {alias} is already registered");
            }

            if (_hostMode.Equals(alias, StringComparison.OrdinalIgnoreCase))
            {
                _serviceCollection.TryAddSingleton<IHostInterface, THostInterface>();
                configureServices(this, _serviceCollection, _configuration);
            }

            return this;
        }

        public DaisyServiceCollection AddChain<TChainBuilder>()
            where TChainBuilder : class, IChainBuilder
        {
            if (!_registeredChains.Add(typeof(TChainBuilder)))
            {
                throw new NotSupportedException($"{typeof(TChainBuilder).Name} is already registered");
            }

            _serviceCollection.AddSingleton<TChainBuilder>();
            _serviceCollection.AddSingleton(s => s.GetRequiredService<TChainBuilder>().BuildChain(s));
            return this;
        }

        public DaisyServiceCollection AddEventHandlerSingleton<TEventHandler>()
            where TEventHandler : class, IDaisyEventHandler
        {
            var handlerType = typeof(TEventHandler);

            var implementedEventHandlerInterfaces = handlerType
                .GetInterfaces()
                .Where(type =>
                    type.IsGenericType &&
                    (type.GetGenericTypeDefinition() == typeof(IDaisyEventHandler<>) ||
                     type.GetGenericTypeDefinition() == typeof(IDaisyEventHandlerAsync<>)));

            _serviceCollection.TryAddSingleton<TEventHandler>();

            // This delegate needs to be explicitly typed to make TryAddEnumerable work.
            Func<IServiceProvider, TEventHandler> serviceFactory = s => s.GetRequiredService<TEventHandler>();

            foreach (var handlerInterface in implementedEventHandlerInterfaces)
            {
                var descriptor = new ServiceDescriptor(handlerInterface, serviceFactory, ServiceLifetime.Singleton);
                _serviceCollection.TryAddEnumerable(descriptor);
            }

            return this;
        }
    }
}