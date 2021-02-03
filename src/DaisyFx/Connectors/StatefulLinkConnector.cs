using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal sealed class StatefulLinkConnector<TInput, TLink, TOutput> : Connector<TInput, TOutput>
        where TLink : StatefulLink<TInput, TOutput>, ILink<TInput,TOutput>
    {
        private readonly TLink _link;

        public StatefulLinkConnector(string name, ConnectorContext context) : base(name, context)
        {
            var instanceContext = new InstanceContext(context.ChainName, Name, context.ChainConfiguration);
            _link = InstanceFactory.Create<TLink>(context.ApplicationServices, instanceContext);

            context.AddInit(_link.InitAsync);
        }

        protected override ValueTask<TOutput> ProcessAsync(TInput input, ChainContext context)
        {
            return _link.Invoke(input, context);
        }

        protected override void Dispose()
        {
            _link.Dispose();
        }
    }
}