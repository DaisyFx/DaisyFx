using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx
{
    public abstract class Source<TOutput> :  ISource<TOutput>
    {
        private readonly InstanceContext _context;

        protected Source()
        {
            _context = InstanceFactory.Context;
        }

        public string Name => _context.Name;

        public string ChainName => _context.ChainName;

        protected T ReadConfiguration<T>() where T : new() => _context.ReadConfiguration<T>();

        public abstract Task ExecuteAsync(SourceNextDelegate<TOutput> next, CancellationToken cancellationToken);

        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose()
        {
        }
    }
}