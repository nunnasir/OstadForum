using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Contract;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Slug = dto.Slug.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
