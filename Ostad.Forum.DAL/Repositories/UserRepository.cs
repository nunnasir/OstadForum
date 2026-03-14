using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class UserRepository(ForumDbContext dbContext)
    : GenericRepository<User>(dbContext), IUserRepository
{
}

