using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaisyFx
{
    public class ChainContext : IReadOnlyChainContext, IAsyncDisposable
    {
        private static readonly Func<object, ValueTask> DisposeDelegate = state =>
        {
            // Prefer async dispose over dispose
            if (state is IAsyncDisposable asyncDisposable)
            {
                return asyncDisposable.DisposeAsync();
            }
            else if (state is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return new();
        };

        private readonly Lazy<ConcurrentDictionary<object, object>> _lazyItemsCollection;
        private readonly IServiceScope _serviceScope;

        public ChainContext(string chainName, IServiceProvider services, CancellationToken cancellationToken)
        {
            Id = Guid.NewGuid();
            ApplicationServices = services;
            Logger = services.GetRequiredService<ILogger<ChainContext>>();
            ChainName = chainName;
            _serviceScope = services.CreateScope();
            _lazyItemsCollection = new Lazy<ConcurrentDictionary<object, object>>();
            EventBroker = new EventBroker(_serviceScope.ServiceProvider);
            CancellationToken = cancellationToken;
            OnComplete = new Stack<(Func<object, ValueTask> callback, object state)>();
        }

        internal Stack<(Func<object, ValueTask> callback, object state)> OnComplete { get; }
        internal IServiceProvider ApplicationServices { get; }
        internal EventBroker EventBroker { get; }
        internal IServiceProvider ScopeServices => _serviceScope.ServiceProvider;

        public string ChainName { get; }
        public Guid Id { get; }
        public CancellationToken CancellationToken { get; }
        public ILogger<ChainContext> Logger { get; }


        public void Set(object key, object value) => _lazyItemsCollection.Value[key] = value;

        /// <exception cref="InvalidCastException">
        /// If the <paramref name="key"/> exist but the stored value isn't of type <typeparamref name="T"/>
        /// </exception>
        public bool TryGet<T>(object key, [MaybeNullWhen(false)] out T value)
        {
            if (!_lazyItemsCollection.IsValueCreated || !_lazyItemsCollection.Value.TryGetValue(key, out var storedValue))
            {
                value = default!;
                return false;
            }

            if (!(storedValue is T storedValueAsT))
            {
                throw new InvalidCastException();
            }

            value = storedValueAsT;
            return true;
        }

        public void RegisterForDispose(IDisposable disposable)
        {
            OnComplete.Push((DisposeDelegate, disposable));
        }

        public void RegisterForDisposeAsync(IAsyncDisposable disposable)
        {
            OnComplete.Push((DisposeDelegate, disposable));
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_lazyItemsCollection.IsValueCreated)
            {
                _lazyItemsCollection.Value.Clear();
            }

            List<Exception>? exceptions = null;
            while (OnComplete.TryPop(out var disposable))
            {
                try
                {
                    await disposable.callback(disposable.state);
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                        exceptions = new();

                    exceptions.Add(e);
                }
            }

            try
            {
                _serviceScope.Dispose();
            }
            catch (Exception e)
            {
                if (exceptions == null)
                    throw;

                exceptions.Add(e);
            }

            switch (exceptions?.Count)
            {
                case 1:
                    throw exceptions[0];
                case > 1:
                    throw new AggregateException(exceptions);
            }
        }
    }
}