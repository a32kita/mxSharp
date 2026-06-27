# AGENTS.md

## Project overview

- Repository name: `mxSharp`
- Root namespace: `MxSharp`
- Demo namespace: `MxSharp.Demo`
- Target framework: `.NET 8`
- Primary transport types:
  - HTTP for OAuth and auxiliary endpoints
  - gRPC for application API calls

## Current architecture

### Root client

- `MxSharpClient`
  - Owns one `HttpClient`
  - Owns one `GrpcChannel`
  - Implements `IDisposable`
  - Disposes both channel and `HttpClient`

### HTTP layer

- `MxSharpHttpTransport`
  - Shared GET/POST functionality
  - JSON and form POST helpers
  - Throws `MixiException` on non-success responses

### Authentication

- `MxSharpOAuthClient`
  - Currently supports OAuth 2.0 client credentials flow
- `MxSharpTokenProvider`
  - Caches tokens
  - Refreshes them on demand

### Posting layer

- `MxSharpGrpcPostClient`
  - Wraps generated `ApplicationService.ApplicationServiceClient`
  - Currently implements `CreatePost` using the official generated gRPC client

## Error handling rule

- API-originated failures must throw `MixiException`
- `MixiException.Message` format must be:
  - `API Error: xxxxxx`

## Coding guidance

- Use English for code comments.
- Keep public API under the `MxSharp` namespace.
- Prefer small focused classes.
- Preserve the shared `HttpClient` ownership model.
- Preserve root-client disposal semantics.

## Future integration steps

1. Add media upload helper and direct upload flow.
2. Add delete post, get users, and DM wrappers.
3. Add streaming client support using the same token provider and channel.
4. Expand response and domain models once actual RPC schemas are available.
5. Consider a dedicated internal layer for generated-type to public-model mapping.