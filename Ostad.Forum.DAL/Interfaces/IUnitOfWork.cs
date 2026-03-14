namespace Ostad.Forum.DAL.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }
    IQuestionRepository Questions { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IAnswerRepository Answers { get; }
    ICommentRepository Comments { get; }
    IVoteRepository Votes { get; }
    IQuestionTagRepository QuestionTags { get; }
    Task<int> SaveChangesAsync();
}

