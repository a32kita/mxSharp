namespace MxSharp;

public sealed class Post
{
    public string PostId { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public string CreatorId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }

    public string? InReplyToPostId { get; set; }

    public string? CommunityId { get; set; }
}