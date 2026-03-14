using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class AnswerRepository(ForumDbContext dbContext)
    : GenericRepository<Answer>(dbContext), IAnswerRepository
{
}

