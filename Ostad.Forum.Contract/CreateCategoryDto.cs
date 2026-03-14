namespace Ostad.Forum.Contract;

public class CreateCategoryDto
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
}
