namespace DaisyFx.Tests.Sources.HttpSource
{
    public class HttpTestChain : ChainBuilder<HttpTestPayload>
    {
        public override string Name { get; } = "Test";

        public override void ConfigureSources(SourceConnectorCollection<HttpTestPayload> sources)
        {
            sources.Add<HttpTestSource>("http");
        }

        public override void ConfigureRootConnector(IConnectorLinker<HttpTestPayload> root)
        {
            root.Map(x => x);
        }
    }
}