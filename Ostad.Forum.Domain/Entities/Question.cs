using System;
using System.Collections.Generic;

namespace Ostad.Forum.Domain.Entities;

public class Question
{
    public int QuestionId { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}

