using System;
using System.Linq;
using DaisyFx.Locking;

namespace DaisyFx
{
    public abstract class ChainBuilder<T> : IChainBuilder
    {
        public abstract string Name { get; }

        public virtual ILockStrategy CreateLockStrategy() => NoLockStrategy.Static;
        public abstract void ConfigureSources(SourceConnectorCollection<T> sources);
        public abstract void ConfigureRootConnector(IConnectorLinker<T> root);

        IChain IChainBuilder.BuildChain(IServiceProvider serviceProvider)
        {
            var lockStrategy = CreateLockStrategy();
            var connectorContext = new ConnectorContext(Name, serviceProvider);
            var rootConnector = ConnectorLinker<T>.CreateAndBuild(ConfigureRootConnector, connectorContext);

            var (connectors, initQueue) = connectorContext.Seal();

            var sourceConnectorCollection = new SourceConnectorCollection<T>(Name, serviceProvider);
            ConfigureSources(sourceConnectorCollection);
            var sourceConnectors = sourceConnectorCollection.ToArray();

            return new Chain<T>(Name, lockStrategy, rootConnector, connectors, sourceConnectors, serviceProvider, initQueue);
        }
    }
}