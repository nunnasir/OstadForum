using System;

namespace Ostad.Forum.Domain.Entities;

public class Vote
{
    public int VoteId { get; set; }
    public int UserId { get; set; }
    public int? QuestionId { get; set; }
    public int? AnswerId { get; set; }
    public int? CommentId { get; set; }
    public VoteType VoteType { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Question? Question { get; set; }
    public Answer? Answer { get; set; }
    public Comment? Comment { get; set; }
}

