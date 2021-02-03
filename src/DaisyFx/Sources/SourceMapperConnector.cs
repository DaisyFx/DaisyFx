using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Sources
{
    public class SourceMapperConnector<TFrom, TTo> : ISourceConnector<TTo>
    {
        private readonly ISourceConnector<TFrom> _wrappedConnector;
        private readonly Func<TFrom, TTo> _mapper;

        public int Index => _wrappedConnector.Index;
        public string Name => _wrappedConnector.Name;

        public SourceMapperConnector(ISourceConnector<TFrom> wrappedConnector, Func<TFrom, TTo> mapper)
        {
            _wrappedConnector = wrappedConnector;
            _mapper = mapper;
        }

        void ISourceConnector<TTo>.Start(ChainExecuteDelegate<TTo> execute)
        {
            Task<ExecutionResult> ExecuteWrapper(TFrom arg, CancellationToken token)
            {
                return execute(_mapper.Invoke(arg), token);
            }

            _wrappedConnector.Start(ExecuteWrapper);
        }

        Task ISourceConnector<TTo>.StopAsync(bool force)
        {
            return _wrappedConnector.StopAsync(force);
        }

        public Task WaitForCompletionAsync()
        {
            return _wrappedConnector.WaitForCompletionAsync();
        }
    }
}