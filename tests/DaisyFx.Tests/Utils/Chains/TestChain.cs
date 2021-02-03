using System;
using DaisyFx.Locking;

namespace DaisyFx.Tests.Utils.Chains
{
    public class TestChain<T> : ChainBuilder<T>
    {
        private readonly string? _name;
        public ILockStrategy? LockStrategy;
        public Action<SourceConnectorCollection<T>>? ConfigureSourcesAction;
        public Action<IConnectorLinker<T>>? ConfigureRootAction;

        public TestChain(string? name = null)
        {
            _name = name;
        }

        public override string Name => _name ?? GetType().Name;

        public override ILockStrategy CreateLockStrategy()
        {
            return LockStrategy ?? base.CreateLockStrategy();
        }

        public override void ConfigureRootConnector(IConnectorLinker<T> root)
        {
            ConfigureRootAction?.Invoke(root);
        }

        public override void ConfigureSources(SourceConnectorCollection<T> sources)
        {
            ConfigureSourcesAction?.Invoke(sources);
        }
    }
}