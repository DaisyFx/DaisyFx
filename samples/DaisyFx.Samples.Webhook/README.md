# Webhook sample

The sample showcases calling a chain through HTTP with a JSON body.

Please take some time to look at [OrderHttpSource](Sources/OrderHttpSource.cs) to understand how incoming requests are authorized and validated.

## Running the sample

1. Run the sample
```
dotnet run
```

2. Call the endpoint with expected payload

Default endpoint is `/daisy/<ChainName>`, this can be changed but  for this sample it's `https://localhost:5001/daisy/order`

Header
```
Authorization: bearer SampleKey
```

Body
```
{
    "OrderDate" : "2012-04-23T18:25:43.511Z",
    "OrderLines": [
        {
            "Article": "Jacket-XL",
            "Quantity": 1
        },
        {
            "Article": "Gloves-S",
            "Quantity": 2
        },
        {
            "Article": "Gloves-M",
            "Quantity": 3
        }
    ]
}
```