using System;
using System.Threading;
using System.Threading.Tasks;

namespace MxSharp;

public sealed class MxSharpTokenProvider : IMxSharpAccessTokenProvider
{
    private readonly MxSharpOAuthClient _oAuthClient;
    private readonly SemaphoreSlim _syncLock = new(1, 1);
    private OAuthTokenResult? _cachedToken;

    public MxSharpTokenProvider(MxSharpOAuthClient oAuthClient)
    {
        _oAuthClient = oAuthClient;
    }

    public async ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && _cachedToken.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
        {
            return _cachedToken.AccessToken;
        }

        await _syncLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_cachedToken is not null && _cachedToken.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return _cachedToken.AccessToken;
            }

            _cachedToken = await _oAuthClient.GetClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);
            _cachedToken.IssuedAt = DateTimeOffset.UtcNow;
            return _cachedToken.AccessToken;
        }
        finally
        {
            _syncLock.Release();
        }
    }
}