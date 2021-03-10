using System;
using DaisyFx.Connectors;

namespace DaisyFx
{
    public class ChainException : Exception
    {
        public ChainException(string message, Exception innerException, IConnector connector)
            : base(message, innerException)
        {
            Connector = connector;
        }

        public IConnector Connector { get; }
    }
}