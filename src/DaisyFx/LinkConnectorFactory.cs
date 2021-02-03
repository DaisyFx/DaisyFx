using System;
using DaisyFx.Connectors;

namespace DaisyFx
{
    internal static class LinkConnectorFactory
    {
        public static IConnector<TInput, TOutput> Create<TInput, TLink, TOutput>(string? name, ConnectorContext context)
            where TLink : ILink<TInput, TOutput>
        {
            name ??= typeof(TLink).Name;

            if(typeof(StatefulLink<TInput,TOutput>).IsAssignableFrom(typeof(TLink)))
            {
                var singletonConnectorType =
                    typeof(StatefulLinkConnector<,,>).MakeGenericType(typeof(TInput), typeof(TLink), typeof(TOutput));

                return (IConnector<TInput, TOutput>) Activator.CreateInstance(singletonConnectorType, name, context)!;
            }

            return new StatelessLinkConnector<TInput, TLink, TOutput>(name, context);
        }
    }
}