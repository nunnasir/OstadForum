using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class QuestionRepository(ForumDbContext dbContext)
    : GenericRepository<Question>(dbContext), IQuestionRepository
{
}

