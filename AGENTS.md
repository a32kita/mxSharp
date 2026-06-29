# AGENTS.md

## Project overview

- Repository name: `mxSharp`
- Root namespace: `MxSharp`
- Demo namespace: `MxSharp.Demo`
- Target framework: `.NET 8`
- Primary transport types:
  - HTTP for OAuth and auxiliary endpoints
  - gRPC for application API calls

## External dependencies in repository

- Official mixi2 proto definitions are included as a git submodule:
  - `external/mixi2-api`
- Do not manually rewrite upstream proto files under:
  - `S:\Public Development\mxSharp\external\mixi2-api`

## Generated code layout

- C# protobuf/gRPC generated files are emitted into:
  - `S:\Public Development\mxSharp\src\mxSharp\Generated`
- Generated files are build artifacts backed by upstream `.proto` files.
- Do not hand-edit generated files unless there is a temporary local debugging need.
- Preferred change flow:
  1. Update `.proto` inputs or project generation settings.
  2. Rebuild.
  3. Consume generated types from handwritten wrapper classes.

## Current architecture

### Root client

- `MxSharpClient`
  - Owns one `HttpClient`
  - Owns one `GrpcChannel`
  - Implements `IDisposable`
  - Disposes both channel and `HttpClient`
  - Exposes HTTP, OAuth, token provider, and posting client entry points

### HTTP layer

- `MxSharpHttpTransport`
  - Shared GET/POST functionality
  - JSON and form POST helpers
  - Throws `MixiException` on non-success responses

### Proto / gRPC integration

- `src/mxSharp/mxSharp.csproj`
  - Uses `Grpc.Tools` for code generation
  - Uses `Protobuf` items with split configuration:
    - `const/v1` => `GrpcServices="None"`
    - `model/v1` => `GrpcServices="None"`
    - `service/application_api/v1` => `GrpcServices="Client"`
    - `service/application_stream/v1` => `GrpcServices="Client"`
    - `service/client_endpoint/v1` => `GrpcServices="None"`
  - Uses `CompileOutputs="false"` and explicit generated output directory behavior
  - Uses `Protobuf_OutputPath=Generated`

- Important generated namespaces currently in use:
  - `Social.Mixi.Application.Service.ApplicationApi.V1`
  - `Social.Mixi.Application.Service.ApplicationStream.V1`
  - `Social.Mixi.Application.Model.V1`
  - `Social.Mixi.Application.Const.V1`

### Authentication

- `MxSharpOAuthClient`
  - Currently supports OAuth 2.0 client credentials flow
- `MxSharpTokenProvider`
  - Caches tokens
  - Refreshes them on demand
  - Supplies bearer tokens for gRPC metadata

### Posting layer

- `MxSharpGrpcPostClient`
  - Wraps generated `ApplicationService.ApplicationServiceClient`
  - Currently implements `CreatePost`, `GetPosts`, and `DeletePost` using the official generated gRPC client
  - Uses handwritten public request/response models and a handwritten `Post` read model
  - Maps `RpcException` to `MixiException`

### Demo app

- `MxSharp.Demo`
  - Interactive console app
  - Prompts for OAuth token URL, gRPC endpoint, client ID, client secret, and optional scope
  - Requests token and calls `CreatePostAsync`

## Endpoint handling note

- As of 2026-06-29, treat the mixi2 OAuth token endpoint and gRPC/API address as configurable values that must be confirmed in the mixi2 Developer Portal.
- Do not assume these endpoints are permanently fixed global constants, even if the currently announced values look stable.
- This repository intentionally keeps endpoint handling configurable because public developer access appears to still be in a trial/early phase.
- In practice the endpoints may become effectively fixed in the future, but current implementation and documentation should continue to direct users to confirm the latest values in the Developer Portal.

## Error handling rule

- API-originated failures must throw `MixiException`
- `MixiException.Message` format must be:
  - `API Error: xxxxxx`

## Current implemented mixi2 API surface

- Implemented:
  - OAuth 2.0 client credentials token acquisition
  - `CreatePost`
  - `GetPosts`
  - `DeletePost`

- Proto-confirmed but not yet wrapped in handwritten public API:
  - `GetUsers`
  - `GetCommunities`
  - `InitiatePostMediaUpload`
  - `GetPostMediaStatus`
  - `SendChatMessage`
  - `GetStamps`
  - `AddStampToPost`
  - `SendDirectMessageToCommunityMember`
  - `SubscribeEvents`

## Coding guidance

- Use English for code comments.
- Keep public API under the `MxSharp` namespace.
- Prefer small focused classes.
- Preserve the shared `HttpClient` ownership model.
- Preserve root-client disposal semantics.
- Prefer wrapping generated types instead of leaking them broadly through the public API.
- If adding new wrappers, use the existing token provider and attach bearer tokens through gRPC `Metadata`.
- If API-originated errors surface as `RpcException`, convert them to `MixiException`.
- Avoid editing generated code directly; adjust proto generation inputs or wrapper code instead.

## Validation guidance for coding agents

- After any change to proto generation or wrapper logic, run:
  - `dotnet build S:\Public Development\mxSharp\mxSharp.slnx -c Release`
- If generation-related errors occur, inspect:
  - `S:\Public Development\mxSharp\src\mxSharp\mxSharp.csproj`
  - `S:\Public Development\mxSharp\src\mxSharp\Generated`
  - `S:\Public Development\mxSharp\external\mixi2-api\proto`
- If runtime posting issues occur, verify:
  - OAuth token endpoint
  - gRPC endpoint
  - client credentials
  - required scopes / app permissions

## Future integration steps

1. Add media upload helper and direct upload flow.
2. Add get users and DM wrappers.
3. Add streaming client support using the same token provider and channel.
4. Expand response and domain models once actual RPC schemas are available.
5. Consider a dedicated internal layer for generated-type to public-model mapping.
