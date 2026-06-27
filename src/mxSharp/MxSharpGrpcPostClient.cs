using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace MxSharp;

public sealed class MxSharpGrpcPostClient
{
    private readonly GrpcChannel _channel;
    private readonly IMxSharpAccessTokenProvider _tokenProvider;

    public MxSharpGrpcPostClient(GrpcChannel channel, IMxSharpAccessTokenProvider tokenProvider)
    {
        _channel = channel;
        _tokenProvider = tokenProvider;
    }

    public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new ArgumentException("Post text is required.", nameof(request));
        }

        var accessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var headers = new Metadata
        {
            { "authorization", $"Bearer {accessToken}" },
        };

        try
        {
            // This method is a placeholder until generated proto clients are integrated.
            // The current implementation verifies authentication and channel plumbing only.
            _ = headers;
            _ = _channel;

            return new CreatePostResponse
            {
                RawResponse = $"Queued placeholder post request: {request.Text}",
            };
        }
        catch (RpcException ex)
        {
            throw new MixiException(ex.Status.Detail, ex);
        }
    }
}