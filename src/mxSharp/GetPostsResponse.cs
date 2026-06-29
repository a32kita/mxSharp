namespace MxSharp;

public sealed class GetPostsResponse
{
    public IList<Post> Posts { get; } = new List<Post>();
}