# Raygun

## Installation

1. Install package

```sh
dotnet add package DaisyFx.Raygun
```

2. Configure Raygun in appsettings

```csharp
"RaygunSettings": {
  "ApiKey": "YOUR_APP_API_KEY"
}
```

3. Register in Daisy

```csharp
services.AddDaisy(hostContext.Configuration, d =>
{
    // ..

    d.AddRaygun();
});
```

## Customize behaviour

## Custom data and tags

Custom data and tags can be added to the error sent to raygun by using an overload for `CreatePayload` or `Send`

1. Create a custom handler
```csharp
public class CustomRaygunExceptionHandler : RaygunExceptionEventHandler
{
    // ..

    // Tags/CustomData for ChainExceptionEvent
    protected override RaygunPayload CreatePayload(ChainExceptionEvent daisyEvent)
    {
        var payload = base.CreatePayload(daisyEvent);

        payload.Tags.Add("Example tag");
        payload.UserCustomData.Add("Example key", "Example value");

        return payload;
    }
    
    // Tags/CustomData for all exception events
    protected override Task Send(RaygunPayload raygunPayload)
    {
        raygunPayload.Tags.Add("Example tag");
        return base.Send(raygunPayload);
    }
}
```

2. Register the handler in Daisy

```csharp
services.AddDaisy(hostContext.Configuration, d =>
{
    // ..

    d.AddRaygun<CustomRaygunExceptionHandler>();
});
```

## Custom raygun client

1. Create a custom raygun client provider
```csharp
public class CustomRaygunClientProvider : RaygunClientProvider
{
    public override RaygunClient GetClient(RaygunSettings settings)
    {
        var client = base.GetClient(settings);
        client.ApplicationVersion = "1.2.0";
        return client;
    }
}
```

2. Register the provider in services

```csharp
services.AddDaisy(hostContext.Configuration, d =>
{
    // ..

    d.AddRaygun();
});

services.AddSingleton<IRaygunClientProvider, CustomRaygunClientProvider>();
```

