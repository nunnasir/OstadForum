namespace Ostad.Forum.Web.Models;

public class QuestionDetailsViewModel
{
    public int QuestionId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
    public List<string> TagNames { get; set; } = new();
    public List<AnswerViewModel> Answers { get; set; } = new();
}
