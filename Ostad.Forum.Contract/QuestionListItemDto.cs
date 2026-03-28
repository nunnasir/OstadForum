namespace Ostad.Forum.Contract;

public class QuestionListItemDto
{
    public int QuestionId { get; set; }
    public string Title { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public IReadOnlyList<int> TagIds { get; set; } = Array.Empty<int>();
    public DateTime CreatedAt { get; set; }
    public int AnswerCount { get; set; }
    public int ViewCount { get; set; }
}
