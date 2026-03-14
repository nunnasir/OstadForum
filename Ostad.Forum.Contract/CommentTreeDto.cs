namespace Ostad.Forum.Contract;

public class CommentTreeDto
{
    public int CommentId { get; set; }
    public string Content { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<CommentTreeDto> Replies { get; set; } = new();
}
