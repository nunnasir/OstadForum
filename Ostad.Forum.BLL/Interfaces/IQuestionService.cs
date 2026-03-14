using Ostad.Forum.Contract;

namespace Ostad.Forum.BLL.Interfaces;

public interface IQuestionService
{
    Task<QuestionDetailsDto?> GetQuestionDetailsAsync(int questionId);
    Task<IReadOnlyList<QuestionListItemDto>> GetLatestQuestionsAsync(int take = 20);
    Task<CreateQuestionDto> GetCreatePageDataAsync();
    Task CreateQuestionAsync(string userEmail, string title, string description, int categoryId, IEnumerable<int> tagIds, IEnumerable<string> newTagNames);
}
