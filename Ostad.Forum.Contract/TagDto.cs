namespace Ostad.Forum.Contract;

public class TagDto
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
}
