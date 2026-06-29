using System.Globalization;
using MxSharp;

namespace MxSharp.Demo;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("mxSharp Demo");
        Console.WriteLine("Enter mixi2 connection settings.");
        Console.WriteLine("This demo currently supports posting one text post.");
        Console.WriteLine("Listing your latest personal posts is not available from the currently integrated proto surface.");

        var tokenEndpoint = ReadRequired("OAuth Token URL");
        var grpcEndpoint = ReadRequired("gRPC API Address (e.g. https://example:443)");
        var clientId = ReadRequired("Client ID");
        var clientSecret = ReadRequired("Client Secret");
        var scope = ReadOptional("Scope (optional)");

        var options = new MxSharpClientOptions
        {
            OAuthTokenEndpoint = new Uri(tokenEndpoint),
            GrpcEndpoint = new Uri(grpcEndpoint),
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scope = scope,
            UserAgent = "mxSharp.Demo/0.1.0",
        };

        using var client = new MxSharpClient(options);

        try
        {
            var token = await client.OAuth.GetClientCredentialsTokenAsync().ConfigureAwait(false);
            Console.WriteLine($"Authenticated. Token type: {token.TokenType}, expires at: {token.ExpiresAt:yyyy/MM/dd HH:mm:ss zzz}");

            Console.WriteLine();
            Console.WriteLine("Ready to create a post.");
            var inputText = ReadOptional("Post text (leave empty to use timestamp test text)");
            var text = string.IsNullOrWhiteSpace(inputText)
                ? $"Test: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}"
                : inputText;

            var response = await client.Posts.CreatePostAsync(new CreatePostRequest
            {
                Text = text,
            }).ConfigureAwait(false);

            Console.WriteLine("Post request completed.");
            Console.WriteLine($"Text: {text}");
            Console.WriteLine($"PostId: {response.PostId ?? "(not returned)"}");
            Console.WriteLine($"RawResponse: {response.RawResponse ?? "(none)"}");
        }
        catch (MixiException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled error: {ex.Message}");
        }
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }
    }

    private static string ReadOptional(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}