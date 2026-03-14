using System.ComponentModel.DataAnnotations;

namespace Ostad.Forum.Web.Areas.Admin.Models;

public class CreateCategoryViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }
}
