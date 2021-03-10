using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx.Tests.Utils
{
    public class TestServiceProvider : IServiceProvider, IAsyncDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public TestServiceProvider(
            Action<IDaisyServiceCollection>? configureDaisy = null,
            Action<IServiceCollection>? configureServices = null,
            IEnumerable<(string key, string value)>? configurations = null)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurations?.ToDictionary(c => c.key, c => c.value))
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            configureServices?.Invoke(serviceCollection);
            serviceCollection.AddDaisy(configuration, collection =>
            {
                configureDaisy?.Invoke(collection);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public ValueTask DisposeAsync()
        {
            return _serviceProvider.DisposeAsync();
        }
    }
}