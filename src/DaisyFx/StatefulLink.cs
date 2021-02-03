using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx
{
    public abstract class StatefulLink<TInput, TOutput> : ILink<TInput,TOutput>
    {
        private readonly InstanceContext _context;

        protected StatefulLink()
        {
            _context = InstanceFactory.Context;
        }

        protected T ReadConfiguration<T>() where T : new() => _context.ReadConfiguration<T>();

        ValueTask<TOutput> ILink<TInput, TOutput>.Invoke(TInput input, ChainContext context) => Invoke(input, context);

        protected abstract ValueTask<TOutput> Invoke(TInput input, ChainContext context);

        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose()
        {
        }

        public virtual ValueTask InitAsync(CancellationToken cancellationToken) => new();
    }
}