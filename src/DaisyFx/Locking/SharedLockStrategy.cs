using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Locking
{
    public class SharedLockStrategy : ILockStrategy
    {
        private readonly SemaphoreSlim _semaphore;

        public SharedLockStrategy(int concurrency)
        {
            _semaphore = new SemaphoreSlim(concurrency);
        }

        public Task RequestLockAsync(in CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(cancellationToken);
        }

        public void ReleaseLock()
        {
            _semaphore.Release();
        }
    }
}