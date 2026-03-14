using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.DAL.Repositories;

namespace Ostad.Forum.DAL.UnitOfWork;

public class UnitOfWork(ForumDbContext dbContext) : IUnitOfWork
{
    private readonly ForumDbContext _dbContext = dbContext;

    private IUserRepository? _users;
    private IQuestionRepository? _questions;
    private ICategoryRepository? _categories;
    private ITagRepository? _tags;
    private IAnswerRepository? _answers;
    private ICommentRepository? _comments;
    private IVoteRepository? _votes;
    private IQuestionTagRepository? _questionTags;

    public IUserRepository Users => _users ??= new UserRepository(_dbContext);
    public IQuestionRepository Questions => _questions ??= new QuestionRepository(_dbContext);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_dbContext);
    public ITagRepository Tags => _tags ??= new TagRepository(_dbContext);
    public IAnswerRepository Answers => _answers ??= new AnswerRepository(_dbContext);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_dbContext);
    public IVoteRepository Votes => _votes ??= new VoteRepository(_dbContext);
    public IQuestionTagRepository QuestionTags => _questionTags ??= new QuestionTagRepository(_dbContext);

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}

