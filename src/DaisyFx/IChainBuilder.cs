using System;

namespace DaisyFx
{
    public interface IChainBuilder
    {
        IChain BuildChain(IServiceProvider serviceProvider);
    }
}