using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Social.Mixi.Application.Service.ApplicationApi.V1;

namespace MxSharp;

public sealed class MxSharpGrpcPostClient
{
    private readonly ApplicationService.ApplicationServiceClient _client;
    private readonly IMxSharpAccessTokenProvider _tokenProvider;

    public MxSharpGrpcPostClient(GrpcChannel channel, IMxSharpAccessTokenProvider tokenProvider)
    {
        _client = new ApplicationService.ApplicationServiceClient(channel);
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
            var grpcRequest = new Social.Mixi.Application.Service.ApplicationApi.V1.CreatePostRequest
            {
                Text = request.Text,
            };

            var grpcResponse = await _client.CreatePostAsync(grpcRequest, headers, cancellationToken: cancellationToken).ResponseAsync.ConfigureAwait(false);

            return new CreatePostResponse
            {
                PostId = grpcResponse.Post?.PostId,
                RawResponse = grpcResponse.Post?.Text,
            };
        }
        catch (RpcException ex)
        {
            throw new MixiException(string.IsNullOrWhiteSpace(ex.Status.Detail) ? ex.Status.StatusCode.ToString() : ex.Status.Detail, ex);
        }
    }
}