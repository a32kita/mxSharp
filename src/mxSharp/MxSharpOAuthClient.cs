using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MxSharp;

public sealed class MxSharpOAuthClient
{
    private readonly MxSharpClientOptions _options;
    private readonly MxSharpHttpTransport _httpTransport;

    public MxSharpOAuthClient(MxSharpClientOptions options, MxSharpHttpTransport httpTransport)
    {
        _options = options;
        _httpTransport = httpTransport;
    }

    public Task<OAuthTokenResult> GetClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        var form = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", _options.ClientId ?? string.Empty),
            new("client_secret", _options.ClientSecret ?? string.Empty),
        };

        if (!string.IsNullOrWhiteSpace(_options.Scope))
        {
            form.Add(new KeyValuePair<string, string>("scope", _options.Scope));
        }

        return _httpTransport.PostFormAsync<OAuthTokenResult>(_options.OAuthTokenEndpoint!.ToString(), form, cancellationToken);
    }
}