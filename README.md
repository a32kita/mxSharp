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
- `CreatePost`, `GetPosts`, and `DeletePost` gRPC calls wired to generated `ApplicationServiceClient`
- Public handwritten `Post` read model used for post retrieval results
- `MixiException` for API error handling
- Demo console application for interactive authentication and posting flow

## Endpoint handling note

- As of 2026-06-29, this project treats the mixi2 OAuth token endpoint and gRPC/API address as configurable values that must be confirmed in the mixi2 Developer Portal.
- This is intentional because the currently available public developer access appears to still be in a trial/early phase, and public documentation does not yet give a strong long-term guarantee that these endpoints are globally fixed forever.
- In practice, the currently announced values may look stable, and they may become effectively fixed in the future, but `mxSharp` deliberately avoids assuming that today.
- When running the demo or configuring the client, always use the endpoint values currently shown in the Developer Portal for your application.

> Note
> Posting is now wired to the generated mixi2 gRPC client. Higher-level wrappers for `CreatePost`, `GetPosts`, and `DeletePost` are implemented. Additional API areas such as media upload, user lookup, and event streaming are still pending.

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

var posts = await client.Posts.GetPostsAsync(new GetPostsRequest
{
    PostIds = { response.PostId! },
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

The demo currently focuses on create-post flow only, but the library also exposes `GetPostsAsync` and `DeletePostAsync` wrappers.

The demo intentionally asks for the token URL and gRPC endpoint instead of hardcoding them. As of 2026-06-29, this repository treats those values as configuration that should be copied from the mixi2 Developer Portal for the target application.

> Note
> The currently integrated proto surface does not expose a direct "list my latest personal posts" API. The demo therefore only supports creating a post at this time.

Run:

```powershell
dotnet run --project S:\Public Development\mxSharp\src\mxSharp.Demo\mxSharp.Demo.csproj
```

## Planned next steps

- Add media upload helper using `InitiatePostMediaUpload` and HTTP upload flow
- Add user lookup and direct message helpers
- Add streaming support wrapper around `SubscribeEvents`
- Expand post/domain models and mapping helpers
