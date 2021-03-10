using System;
using DaisyFx.Events;
using DaisyFx.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx
{
    public interface IDaisyServiceCollection
    {
        public IServiceCollection ServiceCollection { get; }
        public IConfiguration Configuration { get; }

        IDaisyServiceCollection AddHostMode<THostInterface>(string alias,
            Action<IDaisyServiceCollection, IServiceCollection, IConfiguration> configureServices)
            where THostInterface : class, IHostInterface;

        IDaisyServiceCollection AddChain<TChainBuilder>()
            where TChainBuilder : class, IChainBuilder;

        IDaisyServiceCollection AddEventHandlerSingleton<TEventHandler>()
            where TEventHandler : class, IDaisyEventHandler;
    }
}