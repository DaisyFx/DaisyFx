using System;
using System.Collections.Generic;
using DaisyFx.Connectors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx
{
    internal class ConnectorContext
    {
        private bool _sealed;
        private readonly Queue<InitDelegate> _initQueue = new();
        private readonly List<IConnector> _connectors = new();
        private int _depth = -1;

        public ConnectorContext(string chainName, IServiceProvider applicationServices)
        {
            ApplicationServices = applicationServices;
            ChainName = chainName;
            ChainConfiguration = applicationServices.GetRequiredService<IConfiguration>().GetSection(ChainName);
        }

        public IConfiguration ChainConfiguration { get; }
        public IServiceProvider ApplicationServices { get; }
        public string ChainName { get; }

        public (int index, int depth) Add(IConnector connector)
        {
            ThrowIfSealed();
            var index = _connectors.Count;
            _connectors.Add(connector);
            return (index, _depth);
        }

        public IDisposable BeginDepth()
        {
            ThrowIfSealed();
            _depth++;
            return new DisposableAction(DecreaseDepth);
        }

        private void DecreaseDepth()
        {
            ThrowIfSealed();
            _depth--;
        }

        public void AddInit(InitDelegate initFunc)
        {
            ThrowIfSealed();
            _initQueue.Enqueue(initFunc);
        }

        public (IConnector[], Queue<InitDelegate>) Seal()
        {
            ThrowIfSealed();
            _sealed = true;
            return (_connectors.ToArray(), _initQueue);
        }

        private void ThrowIfSealed()
        {
            if(_sealed)
            {
                throw new NotSupportedException($"{nameof(ConnectorContext)} is sealed");
            }
        }
    }
}