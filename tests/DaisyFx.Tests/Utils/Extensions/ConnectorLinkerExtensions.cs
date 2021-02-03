using DaisyFx.Tests.Utils.Connectors;

namespace DaisyFx.Tests.Utils.Extensions
{
    internal static class ConnectorLinkerExtensions
    {
        internal static IConnectorLinker<T> TestInspect<T>(
            this IConnectorLinker<T> parent,
            InitAction? onInit = null,
            ProcessAction<T>? onProcess = null,
            DisposeAction? onDispose = null)
        {
            var next = new TestInspectConnector<T>(onInit, parent.Context)
            {
                ProcessAction = onProcess,
                DisposeAction = onDispose
            };
            return parent.Link(next);
        }

        internal static IConnectorLinker<T> TestDelay<T>(
            this IConnectorLinker<T> parent,
            int millisecondsDelay)
        {
            var next = new DelayConnector<T>(millisecondsDelay, parent.Context);
            return parent.Link(next);
        }
    }
}