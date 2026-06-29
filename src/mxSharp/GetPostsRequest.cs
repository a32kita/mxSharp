namespace MxSharp;

public sealed class GetPostsRequest
{
    public IList<string> PostIds { get; } = new List<string>();
}