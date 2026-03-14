namespace Ostad.Forum.Web.Models;

public class AnswerViewModel
{
    public int AnswerId { get; set; }
    public string Content { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
