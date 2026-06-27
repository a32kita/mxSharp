using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Grpc.Net.Client;

namespace MxSharp;

public sealed class MxSharpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly GrpcChannel _grpcChannel;
    private bool _disposed;

    public MxSharpClient(MxSharpClientOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Options.Validate();

        _httpClient = new HttpClient
        {
            Timeout = Options.Timeout,
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Options.UserAgent);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Http = new MxSharpHttpTransport(_httpClient);
        OAuth = new MxSharpOAuthClient(Options, Http);
        TokenProvider = new MxSharpTokenProvider(OAuth);
        _grpcChannel = GrpcChannel.ForAddress(Options.GrpcEndpoint!, new GrpcChannelOptions
        {
            HttpClient = _httpClient,
        });
        Posts = new MxSharpGrpcPostClient(_grpcChannel, TokenProvider);
    }

    public MxSharpClientOptions Options { get; }

    public MxSharpHttpTransport Http { get; }

    public MxSharpOAuthClient OAuth { get; }

    public IMxSharpAccessTokenProvider TokenProvider { get; }

    public MxSharpGrpcPostClient Posts { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _grpcChannel.Dispose();
        _httpClient.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}