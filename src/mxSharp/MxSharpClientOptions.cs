using System;

namespace MxSharp;

public sealed class MxSharpClientOptions
{
    public Uri? OAuthTokenEndpoint { get; set; }

    public Uri? GrpcEndpoint { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string Scope { get; set; } = string.Empty;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

    public string UserAgent { get; set; } = "mxSharp/0.1.0";

    public void Validate()
    {
        if (OAuthTokenEndpoint is null)
        {
            throw new InvalidOperationException("OAuthTokenEndpoint is required.");
        }

        if (GrpcEndpoint is null)
        {
            throw new InvalidOperationException("GrpcEndpoint is required.");
        }

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException("ClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new InvalidOperationException("ClientSecret is required.");
        }
    }
}