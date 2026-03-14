using Ostad.Forum.Contract;

namespace Ostad.Forum.Web.Models;

public class HomeIndexViewModel
{
    public List<QuestionListItemViewModel> Questions { get; set; } = new();
    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    public IReadOnlyList<TagDto> Tags { get; set; } = Array.Empty<TagDto>();
}

