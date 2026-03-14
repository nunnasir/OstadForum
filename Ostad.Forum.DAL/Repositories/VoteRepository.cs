using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class VoteRepository(ForumDbContext dbContext)
    : GenericRepository<Vote>(dbContext), IVoteRepository
{
}

