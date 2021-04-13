using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx.Connectors
{
    internal sealed class StatelessLinkConnector<TInput, TLink, TOutput> : Connector<TInput, TOutput>
        where TLink : ILink<TInput, TOutput>
    {
        private readonly InstanceContext _context;

        public StatelessLinkConnector(string name, ConnectorContext context) : base(name, context)
        {
            _context = new InstanceContext(context.ChainName, Name, context.ChainConfiguration);

            using var serviceScope = context.ApplicationServices.CreateScope();
            InstanceFactory.Create<TLink>(serviceScope.ServiceProvider, _context).Dispose();
        }

        protected override async ValueTask<TOutput> ProcessAsync(TInput input, ChainContext context)
        {
            var link = InstanceFactory.Create<TLink>(context.ScopeServices, _context);
            context.RegisterForDispose(link);
            return await link.ExecuteAsync(input, context);
        }
    }


}