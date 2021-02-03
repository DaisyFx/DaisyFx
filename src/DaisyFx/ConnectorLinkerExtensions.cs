using System;
using System.Collections.Generic;
using DaisyFx.Connectors;

namespace DaisyFx
{
    public static class ConnectorLinkerExtensions
    {
        public static IConnectorLinker<IReadOnlyList<T>> Each<T>(this IConnectorLinker<IReadOnlyList<T>> parent, Action<IConnectorLinker<T>> buildChain)
        {
            var connector = new EnumerationConnector<T>(buildChain, parent.Context);
            return parent.Link(connector);
        }

        public static IConnectorLinker<T> SubChain<T>(this IConnectorLinker<T> parent, Action<IConnectorLinker<T>> buildChain)
        {
            var connector = new SubChainConnector<T>(buildChain, parent.Context);
            return parent.Link(connector);
        }

        public static void Complete<T>(this IConnectorLinker<T> parent, string reason)
        {
            var connector = new CompletionConnector<T>(reason, parent.Context);
            parent.Link(connector);
        }

        public static IConnectorLinker<T> Conditional<T>(this IConnectorLinker<T> parent, Predicate<T> predicate, Action<IConnectorLinker<T>> buildChain)
        {
            var connector = new ConditionalSubChainConnector<T>(predicate, buildChain, parent.Context);
            parent.Link(connector);
            return connector;
        }

        public static IConnectorLinker<TOutput> Map<T, TOutput>(this IConnectorLinker<T> parent,
            MapperDelegate<T, TOutput> mapper)
        {
            var connector = new MappingConnector<T, TOutput>(mapper, parent.Context);
            parent.Link(connector);
            return connector;
        }
    }
}