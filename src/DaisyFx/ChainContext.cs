using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using DaisyFx.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaisyFx
{
    public class ChainContext : IReadOnlyChainContext, IDisposable
    {
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
        }

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

        void IDisposable.Dispose()
        {
            _serviceScope.Dispose();

            if (_lazyItemsCollection.IsValueCreated)
            {
                foreach (var value in _lazyItemsCollection.Value.Values)
                {
                    if(value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}