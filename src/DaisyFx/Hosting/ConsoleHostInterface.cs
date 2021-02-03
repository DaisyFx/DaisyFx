using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Hosting
{
    internal class ConsoleHostInterface : BackgroundService, IHostInterface
    {
        private readonly ILogger<ConsoleHostInterface> _logger;
        private readonly IChain[] _chains;

        public ConsoleHostInterface(ILogger<ConsoleHostInterface> logger, IEnumerable<IChain> chains)
        {
            _logger = logger;
            _chains = chains.ToArray();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var chain in _chains)
            {
                await chain.InitAsync(cancellationToken);
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"ProcessId: {Process.GetCurrentProcess().Id}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var chain = await SelectChainAsync(_chains, stoppingToken);
                if (chain == null)
                    break;

                await ExecuteChainSourcesAsync(chain, stoppingToken);
            }
            _logger.LogInformation("Stopped");
        }

        private async Task ExecuteChainSourcesAsync(IChain chain, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Starting: {chain.Name}");

            foreach (var connector in chain.Connectors)
            {
                Console.WriteLine($"{connector.Index}" +
                                  new string(' ', connector.Depth * 2) +
                                  $" -> {connector.Name}" +
                                  $" -> {connector.OutputType.Name}");
            }

            try
            {
                chain.StartAllSources();

                while (true)
                {
                    var key = await ReadKey(cancellationToken);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationToken)
            {

            }
            finally
            {
                Console.WriteLine($"Stopping {chain.Name}");
                await chain.StopAllSourcesAsync(false);
                Console.WriteLine($"{chain.Name} stopped");
            }
        }

        private static async Task<IChain?> SelectChainAsync(IReadOnlyList<IChain> chains, CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What chain do you want to trigger");
                for (var i = 0; i < chains.Count; i++)
                {
                    Console.WriteLine($"[{i}] {chains[i].Name}");
                }

                var key = await ReadKey(cancellationToken);

                if (key.Key == ConsoleKey.Escape)
                    break;
                if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                    break;

                if (!int.TryParse(key.KeyChar.ToString(), out var index) || index >= chains.Count || index < 0)
                {
                    Console.WriteLine("Not a valid option");
                    continue;
                }

                return chains[index];
            }

            return default;
        }

        private static Task<ConsoleKeyInfo> ReadKey(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!Console.KeyAvailable)
                {
                    await Task.Delay(100, cancellationToken);
                }

                var key = Console.ReadKey(true);
                return key;
            }, cancellationToken);
        }
    }
}