using System;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal class ConditionalSubChainConnector<TInput> : Connector<TInput, TInput>
    {
        private readonly Predicate<TInput> _predicate;
        private readonly IConnector<TInput> _conditionalConnector;

        public ConditionalSubChainConnector(Predicate<TInput> predicate, Action<IConnectorLinker<TInput>> buildChain,
            ConnectorContext context) : base("Conditional", context)
        {
            _predicate = predicate;
            _conditionalConnector = ConnectorLinker<TInput>.CreateAndBuild(buildChain, context);
        }

        protected override async ValueTask<TInput> ProcessAsync(TInput input, ChainContext context)
        {
            if (_predicate(input))
            {
                await _conditionalConnector.ProcessAsync(input, context);
            }

            return input;
        }

        protected override void Dispose()
        {
            _conditionalConnector.Dispose();
        }
    }
}