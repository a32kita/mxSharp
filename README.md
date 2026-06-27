# mxSharp

mxSharp is a .NET 8 client library scaffold for the mixi2 Developer Platform.

## Target framework

- .NET 8

## Dependencies

| Package | Purpose |
|---|---|
| `Google.Protobuf` | Protocol Buffers runtime for generated message types |
| `Grpc.Net.Client` | gRPC client transport |
| `Grpc.Core.Api` | gRPC abstractions and metadata types |

## Current implementation status

- OAuth 2.0 client credentials token acquisition over HTTP
- Shared `HttpClient` owned by `MxSharpClient`
- Shared `GrpcChannel` owned by `MxSharpClient`
- Common HTTP GET/POST infrastructure
- `MixiException` for API error handling
- Demo console application for interactive authentication and posting flow

> Note
> The current post API implementation is a placeholder until the official mixi2 `.proto` files are integrated and generated C# gRPC clients are added.

## Basic usage

```csharp
using MxSharp;

var options = new MxSharpClientOptions
{
    OAuthTokenEndpoint = new Uri("https://example.com/oauth/token"),
    GrpcEndpoint = new Uri("https://example.com:443"),
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    Scope = "",
};

using var client = new MxSharpClient(options);

var token = await client.OAuth.GetClientCredentialsTokenAsync();

var response = await client.Posts.CreatePostAsync(new CreatePostRequest
{
    Text = "Hello from mxSharp",
});
```

## Error handling

API errors are surfaced as `MixiException`.

```csharp
try
{
    var token = await client.OAuth.GetClientCredentialsTokenAsync();
}
catch (MixiException ex)
{
    Console.WriteLine(ex.Message);
    // Example: API Error: invalid_client
}
```

## Demo application

`mxSharp.Demo` is a .NET 8 console application that:

1. Prompts for OAuth token URL
2. Prompts for gRPC API endpoint
3. Prompts for client credentials
4. Requests an access token
5. Attempts to post `Test: yyyy/MM/dd HH:mm:ss.fff`

Run:

```powershell
dotnet run --project S:\Public Development\mxSharp\src\mxSharp.Demo\mxSharp.Demo.csproj
```

## Planned next steps

- Integrate official mixi2 `.proto` definitions
- Generate C# gRPC client types
- Replace placeholder post implementation with actual RPC calls
- Add streaming support
- Add richer response models