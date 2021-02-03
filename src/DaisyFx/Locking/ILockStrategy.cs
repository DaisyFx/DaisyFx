using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Locking
{
    public interface ILockStrategy
    {
        Task RequestLockAsync(in CancellationToken cancellationToken);
        void ReleaseLock();
    }
}