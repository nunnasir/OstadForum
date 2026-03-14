using Ostad.Forum.Contract;

namespace Ostad.Forum.BLL.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync();
    Task CreateAsync(CreateCategoryDto dto);
}
