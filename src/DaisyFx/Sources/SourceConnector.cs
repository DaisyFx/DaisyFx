using System;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Events;
using DaisyFx.Events.Source;
using DaisyFx.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Sources
{
    public class SourceConnector<TSource, T> : ISourceConnector<T>
        where TSource : ISource<T>
    {
        private readonly string _chainName;
        private readonly IServiceProvider _applicationServices;

        private CancellationTokenSource? _sourceCancellationTokenSource;
        private CancellationTokenSource? _executeCancellationTokenSource;
        private Task _completion = Task.CompletedTask;
        private readonly InstanceContext _context;

        public int Index { get; }
        public string Name { get; }

        public SourceConnector(string chainName, string name, int index, IServiceProvider applicationServices)
        {
            _chainName = chainName;
            _applicationServices = applicationServices;
            Name = name;
            Index = index;
            var chainConfiguration = applicationServices.GetRequiredService<IConfiguration>().GetSection(chainName);
            _context = new InstanceContext(chainName, Name, chainConfiguration);

            using var serviceScope = applicationServices.CreateScope();
            using var source = CreateInstance(serviceScope.ServiceProvider);
        }

        void ISourceConnector<T>.Start(ChainExecuteDelegate<T> execute)
        {
            if (!_completion.IsCompleted)
                throw new NotSupportedException();

            _executeCancellationTokenSource = new CancellationTokenSource();
            _sourceCancellationTokenSource = new CancellationTokenSource();
            var executeCancellationToken = _executeCancellationTokenSource.Token;
            var sourceCancellationToken = _sourceCancellationTokenSource.Token;

            Task<ExecutionResult> ExecuteWrapper(T arg) => execute(arg, executeCancellationToken);

            _completion = Task.Run(async () =>
            {
                var sourceExecutionId = Guid.NewGuid();
                using var serviceScope = _applicationServices.CreateScope();

                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ISourceConnector>>();
                using var chainLogScope = logger.BeginScope(_chainName);
                using var sourceLogScope = logger.BeginScope(Name);

                var eventBroker = new EventBroker(serviceScope.ServiceProvider);

                try
                {
                    using var source = CreateInstance(serviceScope.ServiceProvider);
                    eventBroker.Publish(new SourceStartedEvent(_chainName, Name, Index, sourceExecutionId));
                    await source.ExecuteAsync(ExecuteWrapper, sourceCancellationToken);
                    eventBroker.Publish(new SourceStoppedEvent(_chainName, Name, Index, sourceExecutionId,
                        SourceResult.Completed));
                    logger.SourceCompleted(Name);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == sourceCancellationToken)
                {
                    logger.SourceCanceled(Name, exception);
                    eventBroker.Publish(new SourceStoppedEvent(_chainName, Name, Index, sourceExecutionId, SourceResult.Canceled));
                }
                catch (Exception exception)
                {
                    logger.SourceFatalError(Name, exception);
                    eventBroker.Publish(new SourceStoppedEvent(_chainName, Name, Index, sourceExecutionId,
                        SourceResult.Faulted, exception));
                    await eventBroker.PublishAsync(new SourceExceptionEvent(_chainName, Name, Index, sourceExecutionId,
                        exception));
                }
                finally
                {
                    _executeCancellationTokenSource?.Dispose();
                    _executeCancellationTokenSource = null;
                    _sourceCancellationTokenSource?.Dispose();
                    _sourceCancellationTokenSource = null;
                }
            }, CancellationToken.None);
        }

        Task ISourceConnector<T>.StopAsync(bool force)
        {
            if (force)
            {
                _executeCancellationTokenSource?.Cancel();
            }

            _sourceCancellationTokenSource?.Cancel();
            return _completion;
        }

        public Task WaitForCompletionAsync()
        {
            return _completion;
        }

        private TSource CreateInstance(IServiceProvider services)
            => InstanceFactory.Create<TSource>(services, _context);
    }
}
