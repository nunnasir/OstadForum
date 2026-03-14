namespace Ostad.Forum.Web.Models;

public class QuestionListItemViewModel
{
    public int QuestionId { get; set; }
    public string Title { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int AnswerCount { get; set; }
    public int ViewCount { get; set; }
}

