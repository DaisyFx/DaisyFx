using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace DaisyFx
{
    public class InstanceContext
    {
        private readonly IConfiguration _configuration;

        private readonly Lazy<ConcurrentDictionary<Type, object>> _lazyCache = new();

        public InstanceContext(string chainName, string instanceName, IConfiguration chainConfiguration)
        {
            ChainName = chainName;
            Name = instanceName;
            _configuration = chainConfiguration.GetSection(instanceName);
        }

        public string ChainName { get; }

        public string Name { get; }

        public TConfiguration ReadConfiguration<TConfiguration>() where TConfiguration : new()
        {
            return (TConfiguration)_lazyCache.Value.GetOrAdd(typeof(TConfiguration), (_, arg) =>
            {
                var configuration = new TConfiguration();
                arg.Bind(configuration);
                return configuration;
            }, _configuration);
        }
    }
}