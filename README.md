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
- Official mixi2 `.proto` files integrated via submodule
- Generated C# protobuf and gRPC client code via `Grpc.Tools`
- `CreatePost` gRPC call wired to generated `ApplicationServiceClient`
- `MixiException` for API error handling
- Demo console application for interactive authentication and posting flow

> Note
> Posting is now wired to the generated mixi2 gRPC client. Additional API areas such as media upload, delete, user lookup, and event streaming are still pending higher-level wrappers.

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
5. Prompts for post text and sends one post

> Note
> The currently integrated proto surface does not expose a direct "list my latest personal posts" API. The demo therefore only supports creating a post at this time.

Run:

```powershell
dotnet run --project S:\Public Development\mxSharp\src\mxSharp.Demo\mxSharp.Demo.csproj
```

## Planned next steps

- Add media upload helper using `InitiatePostMediaUpload` and HTTP upload flow
- Add delete post, user lookup, and direct message helpers
- Add streaming support wrapper around `SubscribeEvents`
- Add richer response/domain models and mapping helpers