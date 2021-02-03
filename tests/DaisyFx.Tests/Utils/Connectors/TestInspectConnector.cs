using System.Threading.Tasks;
using DaisyFx.Connectors;

namespace DaisyFx.Tests.Utils.Connectors
{
    internal class TestInspectConnector<T> : Connector<T, T>
    {
        public ProcessAction<T>? ProcessAction { get; set; }
        public DisposeAction? DisposeAction { get; set; }

        public TestInspectConnector(InitAction? onInit, ConnectorContext context) : base(nameof(TestInspectConnector<T>), context)
        {
            if(onInit is {})
                context.AddInit(_ =>
                {
                    onInit(this);
                    return new ValueTask();
                });
        }

        protected override ValueTask<T> ProcessAsync(T input, ChainContext context)
        {
            ProcessAction?.Invoke(input, context);
            return new ValueTask<T>(input);
        }

        protected override void Dispose()
        {
            DisposeAction?.Invoke();
        }
    }

    public delegate void ProcessAction<in T>(T input, ChainContext context);
    public delegate void InitAction(IConnector connector);
    public delegate void DisposeAction();
}