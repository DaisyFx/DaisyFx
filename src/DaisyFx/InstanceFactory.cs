using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx
{
    public static class InstanceFactory
    {
        private static class InstanceFactoryCache<T>
        {
            private static readonly Lazy<ObjectFactory> CachedFactory = new(() =>
                ActivatorUtilities.CreateFactory(typeof(T), Type.EmptyTypes));

            public static T Create(IServiceProvider serviceProvider)
            {
                return (T) CachedFactory.Value(serviceProvider, Array.Empty<object>());
            }
        }

        private static readonly AsyncLocal<InstanceContext> CurrentContext = new();

        public static InstanceContext Context
        {
            get => CurrentContext.Value ?? throw new Exception();
            internal set => CurrentContext.Value = value;
        }

        public static T Create<T>(IServiceProvider serviceProvider, InstanceContext context)
        {
            Context = context;
            return InstanceFactoryCache<T>.Create(serviceProvider);
        }
    }
}