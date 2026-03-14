using Ostad.Forum.Contract;

namespace Ostad.Forum.BLL.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<TagDto>> GetAllAsync();
}
