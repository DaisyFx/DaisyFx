using System;
using DaisyFx.Connectors;

namespace DaisyFx
{
    public class ConnectorException : Exception
    {
        public ConnectorException(string message, Exception innerException, IConnector connector)
            : base(message, innerException)
        {
            Connector = connector;
        }

        public IConnector Connector { get; }
    }
}