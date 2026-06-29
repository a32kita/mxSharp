using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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
        var headers = CreateAuthorizationHeaders(accessToken);

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
                Post = grpcResponse.Post is null ? null : MapPost(grpcResponse.Post),
            };
        }
        catch (RpcException ex)
        {
            throw new MixiException(string.IsNullOrWhiteSpace(ex.Status.Detail) ? ex.Status.StatusCode.ToString() : ex.Status.Detail, ex);
        }
    }

    public async Task<GetPostsResponse> GetPostsAsync(GetPostsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.PostIds.Count == 0)
        {
            throw new ArgumentException("At least one post ID is required.", nameof(request));
        }

        if (request.PostIds.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Post IDs cannot contain null, empty, or whitespace values.", nameof(request));
        }

        var accessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var headers = CreateAuthorizationHeaders(accessToken);

        try
        {
            var grpcRequest = new Social.Mixi.Application.Service.ApplicationApi.V1.GetPostsRequest();
            grpcRequest.PostIdList.AddRange(request.PostIds);

            var grpcResponse = await _client.GetPostsAsync(grpcRequest, headers, cancellationToken: cancellationToken).ResponseAsync.ConfigureAwait(false);
            var response = new GetPostsResponse();

            foreach (var post in grpcResponse.Posts)
            {
                response.Posts.Add(MapPost(post));
            }

            return response;
        }
        catch (RpcException ex)
        {
            throw new MixiException(string.IsNullOrWhiteSpace(ex.Status.Detail) ? ex.Status.StatusCode.ToString() : ex.Status.Detail, ex);
        }
    }

    public async Task<DeletePostResponse> DeletePostAsync(DeletePostRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.PostId))
        {
            throw new ArgumentException("Post ID is required.", nameof(request));
        }

        var accessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var headers = CreateAuthorizationHeaders(accessToken);

        try
        {
            var grpcRequest = new Social.Mixi.Application.Service.ApplicationApi.V1.DeletePostRequest
            {
                PostId = request.PostId,
            };

            var grpcResponse = await _client.DeletePostAsync(grpcRequest, headers, cancellationToken: cancellationToken).ResponseAsync.ConfigureAwait(false);

            return new DeletePostResponse
            {
                Deleted = grpcResponse.Deleted,
            };
        }
        catch (RpcException ex)
        {
            throw new MixiException(string.IsNullOrWhiteSpace(ex.Status.Detail) ? ex.Status.StatusCode.ToString() : ex.Status.Detail, ex);
        }
    }

    private static Metadata CreateAuthorizationHeaders(string accessToken)
    {
        return new Metadata
        {
            { "authorization", $"Bearer {accessToken}" },
        };
    }

    private static Post MapPost(Social.Mixi.Application.Model.V1.Post grpcPost)
    {
        return new Post
        {
            PostId = grpcPost.PostId,
            IsDeleted = grpcPost.IsDeleted,
            CreatorId = grpcPost.CreatorId,
            Text = grpcPost.Text,
            CreatedAt = grpcPost.CreatedAt is null ? null : DateTimeOffset.FromUnixTimeSeconds(grpcPost.CreatedAt.Seconds).ToOffset(TimeSpan.Zero).AddTicks(grpcPost.CreatedAt.Nanos / 100),
            InReplyToPostId = grpcPost.InReplyToPostId,
            CommunityId = grpcPost.CommunityId,
        };
    }
}