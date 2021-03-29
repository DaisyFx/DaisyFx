using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Connectors;
using DaisyFx.Events.Chain;
using DaisyFx.Locking;
using DaisyFx.Logging;
using DaisyFx.Sources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaisyFx
{
    public sealed class Chain<T> : IChain
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Queue<InitDelegate> _initQueue;
        private readonly ISourceConnector<T>[] _sourceConnectors;
        private readonly IConnector<T> _root;
        private readonly ILogger<IChain> _logger;

        public Chain(string name, ILockStrategy lockStrategy, IConnector<T> rootConnector,
            IConnector[] connectors, ISourceConnector<T>[] sourceConnectors, IServiceProvider serviceProvider,
            Queue<InitDelegate> initQueue)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<IChain>>();
            _initQueue = initQueue;
            Name = name;
            LockStrategy = lockStrategy;
            _root = rootConnector;
            Connectors = connectors;
            _sourceConnectors = sourceConnectors;
        }

        public ILockStrategy LockStrategy { get; }

        public string Name { get; }

        public IReadOnlyList<IConnector> Connectors { get; }

        public IReadOnlyList<ISourceConnector> Sources => _sourceConnectors;

        public async ValueTask InitAsync(CancellationToken cancellationToken)
        {
            while (_initQueue.TryDequeue(out var initFunc))
            {
                await initFunc(cancellationToken);
            }
        }

        public void StartAllSources()
        {
            foreach (var sourceConnector in _sourceConnectors)
            {
                sourceConnector.Start(ExecuteAsync);
            }
        }

        public Task StopAllSourcesAsync(bool force = false)
        {
            return Task.WhenAll(_sourceConnectors.Select(s => s.StopAsync(force)));
        }

        public void StartSource(int sourceIndex)
        {
            _sourceConnectors[sourceIndex].Start(ExecuteAsync);
        }

        public Task StopSourceAsync(int sourceIndex, bool force = false)
        {
            return _sourceConnectors[sourceIndex].StopAsync(force);
        }

        internal async Task<ExecutionResult> ExecuteAsync(T input, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.RequestingLock();
            await LockStrategy.RequestLockAsync(cancellationToken);
            _logger.Executing();

            await using var context = new ChainContext(Name, _serviceProvider, cancellationToken);

            try
            {
                context.EventBroker.Publish(new ChainExecutionStartedEvent(context));

                var startTickCount = Environment.TickCount64;
                var result = await ExecuteRootConnector(input, context);
                var duration = TimeSpan.FromMilliseconds(Environment.TickCount64 - startTickCount);

                LogResult(result);

                if (result is {Status: ExecutionResultStatus.Faulted, Exception: {}})
                {
                    await context.EventBroker.PublishAsync(new ChainExceptionEvent(context, result.Exception));
                }

                context.EventBroker.Publish(new ChainExecutionResultEvent(context, duration, result));

                LockStrategy.ReleaseLock();

                return result;
            }
            catch (Exception e)
            {
                try
                {
                    _logger.LogCritical(e, "Critical failure");
                }
                finally
                {
                    Environment.Exit(2013);
                }

                throw;
            }
        }

        private void LogResult(ExecutionResult result)
        {
            switch (result)
            {
                case {Status: ExecutionResultStatus.Completed}:
                    _logger.ExecutionCompleted();
                    break;
                case {Status: ExecutionResultStatus.Faulted, Exception: {} exception}:
                    _logger.ExecutionFaulted(exception);
                    break;
            }
        }

        private async ValueTask<ExecutionResult> ExecuteRootConnector(T input, ChainContext context)
        {
            try
            {
                await _root.ProcessAsync(input, context);
                return ExecutionResult.Completed;
            }
            catch(ChainException exception)
            {
                return ExecutionResult.Faulted(exception);
            }
        }

        public void Dispose()
        {
            _root.Dispose();
        }
    }
}