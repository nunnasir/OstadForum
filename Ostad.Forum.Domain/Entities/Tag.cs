using System;
using System.Collections.Generic;

namespace Ostad.Forum.Domain.Entities;

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
}

