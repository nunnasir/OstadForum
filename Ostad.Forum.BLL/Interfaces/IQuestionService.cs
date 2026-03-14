using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Interfaces;

public interface IQuestionService
{
    Task<Question?> GetByIdAsync(int questionId);
    Task<IReadOnlyList<Question>> GetLatestQuestionsAsync(int take = 20);
    Task CreateQuestionAsync(string userEmail, string title, string description, int categoryId, IEnumerable<int> tagIds, IEnumerable<string> newTagNames);
}

