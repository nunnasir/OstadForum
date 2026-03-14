using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Contract;
using Ostad.Forum.DAL.Interfaces;

namespace Ostad.Forum.BLL.Services;

public class TagService(IUnitOfWork unitOfWork) : ITagService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<TagDto>> GetAllAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        return tags.Select(t => new TagDto
        {
            TagId = t.TagId,
            Name = t.Name,
            Slug = t.Slug
        }).ToList();
    }
}
