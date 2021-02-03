using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DaisyFx.TestHelpers
{
    public class SourceTestRunner<TSource, T>
        where TSource : ISource<T>
    {
        private readonly IServiceProvider _services;
        private readonly string _name;
        private readonly IConfiguration _configuration;

        public SourceTestRunner(IServiceProvider services, params (string key, string value)[] configurationValues)
        {
            _services = services;
            _name = typeof(TSource).Name;

            var configurationKeyValuePairs = configurationValues.Select(
                kv => new KeyValuePair<string, string>($"{_name}:{kv.key}", kv.value));

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationKeyValuePairs).Build();
        }

        public async Task Execute(SourceNextDelegate<T> next, CancellationToken ct)
        {
            using var source = InstanceFactory.Create<TSource>(_services, new("test", _name, _configuration));
            await source.ExecuteAsync(next, ct);
        }
    }
}