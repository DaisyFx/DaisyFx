using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Locking
{
    public class NoLockStrategy : ILockStrategy
    {
        public static readonly NoLockStrategy Static = new NoLockStrategy();

        public Task RequestLockAsync(in CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void ReleaseLock()
        {

        }
    }
}