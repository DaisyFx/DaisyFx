using DaisyFx.Samples.Webhook.Links;
using DaisyFx.Samples.Webhook.Model;
using DaisyFx.Samples.Webhook.Sources;

namespace DaisyFx.Samples.Webhook
{
    public class OrderChain : ChainBuilder<Order>
    {
        public override string Name { get; } = "Order";

        public override void ConfigureSources(SourceConnectorCollection<Order> sources)
        {
            sources.Add<OrderHttpSource>("Http");
        }

        public override void ConfigureRootConnector(IConnectorLinker<Order> root)
        {
            root.Link<PrintOrderToConsole, Order>();
        }
    }
}