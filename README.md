<h3 align="center">
  <img src="logo_daisy.svg" alt="DaisyFx" width="500" /><br />
  <a href="#Getting-started">Getting started</a>
  <span> - </span>
  <a href="#Concepts">Concepts</a>
  <span> - </span>
  <a href="#Available-Connectors">Connectors</a>
  <span> - </span>
  <a href="#Samples">Samples</a>
</h3>

Daisy is a framework for building data integrations in a structured manner with minimal overhead. Integrations are implemented as series of steps where the result of each step is the input of the next. These steps are composed into strongly-typed chains using a fluent interface.

Example of a chain:

```csharp
public class ExampleChain : ChainBuilder<Signal>
{
    public override string Name { get; } = "ExampleChain";

    public override void ConfigureSources(SourceConnectorCollection<Signal> sources)
    {
        sources.Add<NCrontabSource>("Cron");
    }

    public override void ConfigureRootConnector(IConnectorLinker<Signal> root)
    {
        root.Link<GetDataRecords, DataRecord[]>()
            .Map(MapRecords)
            .Each(each => each
                .Link<PersistRecord, RecordModel>()
                .Link<SendEmail, RecordModel>()
            );
    }

    private static RecordModel[] MapRecords(DataRecord[] input)
    {
        // …
    }
}
```

## Getting started

1. Install package

```sh
dotnet add package DaisyFx
```

2. Define links

```csharp
public class GenerateGuid : StatelessLink<Signal, Guid>
{
    protected override ValueTask<Guid> ExecuteAsync(Signal input, ChainContext context)
    {
        return new(Guid.NewGuid());
    }
}
```

```csharp
public class WriteToConsole : StatelessLink<Guid, Signal>
{
    protected override ValueTask<Signal> ExecuteAsync(Guid input, ChainContext context)
    {
        Console.WriteLine(input);
        return Signal.Static;
    }
}
```


3. Define chain

```csharp
public class ExampleChain : ChainBuilder<Signal>
{
    public override string Name { get; } = "ExampleChain";

    public override void ConfigureSources(SourceConnectorCollection<Signal> sources)
    {
        sources.Add<TriggerOnStartupSource>("TriggerOnStartup");
    }

    public override void ConfigureRootConnector(IConnectorLinker<Signal> root)
    {
        root.Link<GenerateGuid, Guid>()
            .Link<WriteToConsole, Signal>();
    }
}
```

4. Register in ServiceCollection

```csharp
services.AddDaisy(Configuration, d =>
{
    d.AddChain<ExampleChain>();
});
```

## Concepts

- [Chain](#Link)
- [Source](#Source)
- [Connector](#Connector)
- [ChainContext](#ChainContext)
- [LockStrategy](#LockStrategy)
- [Signal](#Signal)

### Chain

A chain is an integration defined as a data flow. It is built using connectors which defines the control flow and processing steps for handling data. It supports registering multiple sources of execution. A LockStrategy can be specified to cap the number of simultaneous executions.

It's defined by inheriting `ChainBuilder<T>` where `T` is the expected input from sources that want to trigger the chain.

### Source

A source is a starting point for a chain. It will implement logic for when to trigger a chain and can pass a payload along with each run. One example is the NCronTabSource which starts the chain based on a defined cron schedule. If the payload of the source doesn't match the input of the chain, a mapping function can be provided.

#### Implementation

```csharp
public class ExampleSource : Source<DateTime>
{
    public override async Task ExecuteAsync(SourceNextDelegate<DateTime> next, CancellationToken cancellationToken)
    {
        // Directly executes the chain once on startup with the current date as payload
        await next(DateTime.Now);
    }
}
```

#### Usage

```csharp
// Ordinary registration when the source output matches the ChainBuilder
sources.Add<ExampleSource>("ConfigurationName");

// Registration with a mapping function (DateTime -> int) if used in a ChainBuilder<int>
Sources.Add<ExampleSource,int>("ConfigurationName", dt => dt.Year);
```

### Connector

Connectors are the steps that form a chain. Created using a fluent builder, they describe how data will flow and be processed.

[List of available connectors](#Available-Connectors)

### ChainContext

The ChainContext is a control interface that provides access to a per-execution scoped data collection, utilities and manipulation of the execution itself. The data collection allows steps to communicate without having to flow the data as input / output through the chain. The collection items are discarded and disposed after the execution is done.

#### Set

Adds or updates an item with a specified key and a value.

```csharp
context.Set("Foo", "Bar");
```

#### TryGet

Gets the value associated with the specified key.

```csharp
if(context.TryGet("Foo", out string fooValue))
{
    context.Logger.LogInformation(fooValue);
}
```

#### Logger

Access an ILogger to perform logging.

```csharp
context.Logger.LogInformation("Example log message");
```

### LockStrategy

Used to throttle simultaneous execution of a chain, preventing it from becoming overloaded. Without an explicit lock strategy set, there is no defined limit on concurrent executions.

#### SharedLockStrategy

Callers share a lock with a fixed number of allowed concurrent executions.

```csharp
// This allows 1 concurrent execution of the chain
public override ILockStrategy CreateLockStrategy()
{
    return new SharedLockStrategy(concurrency: 1);
}
```

### Signal

Used to represent a message without any data flowing through a chain.

## Available Connectors

- [Link](#Link)
- [Map](#Map)
- [SubChain](#SubChain)
- [If](#If)
- [Each](#Each)

### Link

A link is re-usable asynchronous step, its `ExecuteAsync` method is called with the input from the previous step. The link then returns the value that should be passed to the next step in the chain.

Links can be either stateful or stateless:
- `StatefulLink` is instantiated once and the same instance is used for each execution, therefore they can hold state between executions but has to be thread-safe
    - More performant if no scoped services are used
    - Only singleton and transient services can be requested in the constructor
- `StatelessLink` is instantiated in each execution scope and can't hold state between executions
    - Can request singleton, transient and scoped services in ctor

#### Definition

```csharp
public class IntToString : StatefulLink<int, string>
{
    private long _totalValueSeen = 0;
    
    protected override ValueTask ExecuteAsync(int input, ChainContext context)
    {
        var newTotal = Interlocked.Add(ref _totalValueSeen, input);
        context.Logger.LogInformation($"I've seen a total value of {newTotal}");
        return input.ToString();
    }
}
```

#### Usage

```csharp
root.Link<IntToString, string>();
```

### Map

Run a `Func<TInput, TOutput>` and use the returned value as input for continuation.

```csharp
root.Map(input =>
{
    return input * 2;
})
```

### SubChain

Allows you to branch the chain to a separate data flow. SubChains are executed sequentially, in other words, the parent chain will wait for the sub-chain to be completed before continuing.

```csharp
root.SubChain(subChain => subChain
        .Map(DivideByTwo)
        .Link<IntToString, string>()
        .Link<ConsoleWriteLine, string>();
    )
    .Map(MultiplyByTwo)
    .Link<IntToString, string>()
    .Link<ConsoleWriteLine, string>();
```

SubChains also provides a way to move isolated parts of a chain into separate static classes or methods.

```csharp
root.SubChain(DivisionSubChain.Build)
    .SubChain(FactorSubChain.Build)

// DivisionSubChain.cs
public static class DivisionSubChain
{
    public static void Build(IChainBuilder<int> builder)
    {
        builder
            .Map(DivideByTwo)
            .Action(WriteToConsole);
    }
}
```

### If

A control flow which executes the sub-chain if the predicate evaluates to true.

```csharp
root.If(IsDivisibleBy3, then => then
        .Map(_ => "Value was divisible by 3")
        .Link<ConsoleWriteLine, string>());
```

### Each

Like [SubChain](#SubChain) but takes a collection as input and executes the sub-chain for each value.

```csharp
root.Map(FilterNegativeNumbers)
    .Each(each => each
        .Map(MultiplyByTwo)
        .Link<ConsoleWriteLine, string>());
```

## Samples

- [KitchenSink](./samples/DaisyFx.Samples.KitchenSink)
- [Incoming webhook](./samples/DaisyFx.Samples.Webhook)
- [LoanBroker](./samples/DaisyFx.Samples.LoanBroker)
- [LoanBroker.Tests](./samples/DaisyFx.Samples.LoanBroker.Tests)
