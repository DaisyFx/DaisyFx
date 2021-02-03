using System;
using System.Threading.Tasks;

namespace DaisyFx
{
    public interface ILink<in TInput, TOutput> : IDisposable
    {
        ValueTask<TOutput> Invoke(TInput input, ChainContext context);
    }
}