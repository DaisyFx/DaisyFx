using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal class EnumerationConnector<T> : Connector<IReadOnlyList<T>, IReadOnlyList<T>>
    {
        private readonly IConnector<T> _connector;

        public EnumerationConnector(Action<IConnectorLinker<T>> buildChain, ConnectorContext context) : base("Enumerate", context)
        {
            _connector = ConnectorLinker<T>.CreateAndBuild(buildChain, context);
        }

        protected override async ValueTask<IReadOnlyList<T>> ProcessAsync(IReadOnlyList<T> input, ChainContext context)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < input.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                await _connector.ProcessAsync(input[i], context);
            }

            return input;
        }
    }
}