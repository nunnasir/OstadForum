using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class CategoryRepository(ForumDbContext dbContext)
    : GenericRepository<Category>(dbContext), ICategoryRepository
{
}

