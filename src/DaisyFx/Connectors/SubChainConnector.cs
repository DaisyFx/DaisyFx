using System;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal class SubChainConnector<TInput> : Connector<TInput, TInput>
    {
        private readonly IConnector<TInput> _connector;

        public SubChainConnector(Action<IConnectorLinker<TInput>> buildChain, ConnectorContext context) : base("SubChain", context)
        {
            _connector = ConnectorLinker<TInput>.CreateAndBuild(buildChain, context);
        }

        protected override async ValueTask<TInput> ProcessAsync(TInput input, ChainContext context)
        {
            var onCompleteCountBefore = context.OnComplete.Count;

            await _connector.ProcessAsync(input, context);

            for (var _ = context.OnComplete.Count; _ > onCompleteCountBefore; _--)
            {
                var (callback, state) = context.OnComplete.Pop();
                await callback(state);
            }

            return input;
        }

        protected override void Dispose()
        {
            _connector.Dispose();
        }
    }
}