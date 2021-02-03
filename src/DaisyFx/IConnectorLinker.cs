using DaisyFx.Connectors;

namespace DaisyFx
{
    public interface IConnectorLinker<out TOutput>
    {
        internal ConnectorContext Context { get; }
        internal IConnectorLinker<TNextValue> Link<TNextValue>(IConnector<TOutput, TNextValue> connector);

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// Custom name for <typeparamref name="TLink"/>, defaults to <code>typeof(TLink).Name</code>
        /// </param>
        /// <typeparam name="TLink"></typeparam>
        /// <typeparam name="TNextValue">The type of value that <typeparamref name="TLink"/> produces</typeparam>
        /// <returns></returns>
        IConnectorLinker<TNextValue> Link<TLink, TNextValue>(string? name = null) where TLink : ILink<TOutput, TNextValue>;
    }
}