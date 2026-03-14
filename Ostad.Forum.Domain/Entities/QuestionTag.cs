namespace Ostad.Forum.Domain.Entities;

public class QuestionTag
{
    public int QuestionTagId { get; set; }
    public int QuestionId { get; set; }
    public int TagId { get; set; }

    public Question Question { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

