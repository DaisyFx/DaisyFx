using System.Threading.Tasks;

namespace DaisyFx.Sources
{
    public interface ISourceConnector
    {
        int Index { get; }
        string Name { get; }
        Task WaitForCompletionAsync();
    }

    public interface ISourceConnector<out T> : ISourceConnector
    {
        internal void Start(ChainExecuteDelegate<T> execute);
        internal Task StopAsync(bool force = false);
    }
}