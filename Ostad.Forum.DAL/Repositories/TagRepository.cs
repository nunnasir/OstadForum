using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class TagRepository(ForumDbContext dbContext)
    : GenericRepository<Tag>(dbContext), ITagRepository
{
}

