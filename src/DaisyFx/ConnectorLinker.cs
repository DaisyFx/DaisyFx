using System;
using DaisyFx.Connectors;

namespace DaisyFx
{
    internal class ConnectorLinker<T> : IConnectorLinker<T>
    {
        private readonly ConnectorContext _context;

        private ConnectorLinker(ConnectorContext context)
        {
            _context = context;
        }

        private IConnector<T> Connector { get; set; } = SinkConnector<T>.Static;

        ConnectorContext IConnectorLinker<T>.Context => _context;

        IConnectorLinker<TNextValue> IConnectorLinker<T>.Link<TNextValue>(IConnector<T, TNextValue> connector)
        {
            if (Connector != SinkConnector<T>.Static)
            {
                throw new NotSupportedException("Continuation already configured");
            }

            Connector = connector;
            return connector;
        }

        IConnectorLinker<TNextValue> IConnectorLinker<T>.Link<TLink, TNextValue>(string? name)
        {
            if (Connector != SinkConnector<T>.Static)
            {
                throw new NotSupportedException("Continuation already configured");
            }

            var connector = LinkConnectorFactory.Create<T, TLink, TNextValue>(name, _context);
            Connector = connector;
            return connector;
        }

        public static IConnector<T> CreateAndBuild(Action<IConnectorLinker<T>> buildChain,
            ConnectorContext connectorContext)
        {
            var linker = new ConnectorLinker<T>(connectorContext);
            using var _ = connectorContext.BeginDepth();
            buildChain(linker);
            return linker.Connector;
        }
    }
}