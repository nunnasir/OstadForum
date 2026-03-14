using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL.Repositories;

public class CommentRepository(ForumDbContext dbContext)
    : GenericRepository<Comment>(dbContext), ICommentRepository
{
}

