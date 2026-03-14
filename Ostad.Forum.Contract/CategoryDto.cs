namespace Ostad.Forum.Contract;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
