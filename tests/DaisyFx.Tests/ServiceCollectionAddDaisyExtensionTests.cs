using System;
using System.Collections.Generic;
using System.Linq;
using DaisyFx.Hosting;
using DaisyFx.Tests.Utils.Chains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DaisyFx.Tests
{
    public class ServiceCollectionAddDaisyExtensionTests
    {
        private static IConfiguration CreateConfiguration(params (string key, string value)[] keyValues)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(keyValues.Select(kv => new KeyValuePair<string, string>(kv.key, kv.value)))
                .Build();
        }

        private static ServiceProvider CreateServiceProvider(Action<ServiceCollection> builder)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            builder(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }

        [Theory]
        [InlineData("console", typeof(ConsoleHostInterface))]
        [InlineData("service", typeof(ServiceHostInterface))]
        public void AddDaisy_WithModeConfiguration_RegistersHost(string mode, Type expectedHostType)
        {
            var configuration = CreateConfiguration(("Daisy:Mode", mode));
            var provider = CreateServiceProvider(serviceCollection =>
            {
                serviceCollection.AddDaisy(configuration, _ => { });
            });

            var hostedServices = provider.GetServices<IHostedService>();

            Assert.Contains(hostedServices, service => service.GetType() == expectedHostType);
        }

        [Fact]
        public void AddDaisy_AddChain_RegistersChainAsSingleton()
        {
            var configuration = CreateConfiguration();
            var serviceCollection = new ServiceCollection()
                .AddDaisy(configuration, d =>
                {
                    d.AddChain<TestChain<Signal>>();
                });

            Assert.Single(serviceCollection,
                descriptor => descriptor.ServiceType == typeof(IChain) &&
                              descriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void AddDaisy_AddChain_RegistersEnumerableIChain()
        {
            var configuration = CreateConfiguration();
            var serviceCollection = new ServiceCollection()
                .AddDaisy(configuration, d =>
                {
                    d.AddChain<TestChain<Signal>>();
                    d.AddChain<TestChain<string>>();
                });

            var iChainRegistrations = serviceCollection.Where(descriptor => descriptor.ServiceType == typeof(IChain))
                .ToArray();

            Assert.Equal(2, iChainRegistrations.Length);
        }

        [Fact]
        public void AddDaisy_AddHostModeDuplicate_ThrowsNotSupported()
        {
            var configuration = CreateConfiguration();
            Assert.Throws<NotSupportedException>(() =>
            {
                new ServiceCollection()
                    .AddDaisy(configuration, d =>
                    {
                        d.AddHostMode<ConsoleHostInterface>("console", (_, _, _) => { });
                        d.AddHostMode<ConsoleHostInterface>("console", (_, _, _) => { });
                    });
            });
        }

        [Fact]
        public void AddDaisy_AddHostMode_RegistersHostAndServices()
        {
            const string verification = nameof(verification);
            var configuration = CreateConfiguration(("daisy:mode", "console"));

            var services = new ServiceCollection()
                .AddLogging()
                .AddDaisy(configuration, d =>
                {
                    d.AddHostMode<ConsoleHostInterface>("console", (_, serviceCollection, _) =>
                    {
                        serviceCollection.AddSingleton(verification);
                    });
                }).BuildServiceProvider();

            Assert.Equal(typeof(ConsoleHostInterface), services.GetRequiredService<IHostInterface>().GetType());
            Assert.Equal(typeof(ConsoleHostInterface), services.GetRequiredService<IHostedService>().GetType());
            Assert.Equal(verification, services.GetRequiredService<string>());
        }
    }
}