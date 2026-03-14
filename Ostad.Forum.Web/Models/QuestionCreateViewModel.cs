using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ostad.Forum.Web.Models;

public class QuestionCreateViewModel
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Tags")]
    public List<int> SelectedTagIds { get; set; } = new();

    [Display(Name = "New Tags (comma separated)")]
    public string? NewTags { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Tags { get; set; } = Enumerable.Empty<SelectListItem>();
}

