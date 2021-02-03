using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx.TestHelpers
{
    public abstract class LinkTestRunnerBase<TLink, TInput, TOutput> where TLink : ILink<TInput, TOutput>
    {
        private readonly string _chainName;
        private readonly string _linkName;
        private readonly IConfiguration _configuration;
        protected readonly IServiceProvider Services;

        protected LinkTestRunnerBase(ConfigureServicesDelegate? services = null,
            (string key, string value)[]? configuration = null,
            string? chainName = null)
        {
            _chainName = chainName ?? "LinkTestRunnerChain";
            _linkName = typeof(TLink).Name;
            _configuration = BuildConfiguration(configuration ?? Array.Empty<(string key, string value)>());
            Services = BuildServiceProvider(services);
        }

        protected ChainContext CreateContext(IServiceProvider services, CancellationToken ct)
        {
            return new(_chainName, services, ct);
        }

        protected ILink<TInput, TOutput> CreateLink(IServiceProvider services)
        {
            return InstanceFactory.Create<TLink>(services, new(_chainName, _linkName, _configuration));
        }

        private IConfigurationRoot BuildConfiguration((string key, string value)[] configuration)
        {
            var configurationValues = configuration.ToDictionary(
                kvp => $"{_linkName}:{kvp.key}",
                kvp => kvp.value);

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();
        }

        private static IServiceProvider BuildServiceProvider(ConfigureServicesDelegate? services)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            services?.Invoke(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
    }

    public delegate void ConfigureServicesDelegate(IServiceCollection services);
}