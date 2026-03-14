namespace Ostad.Forum.BLL.Interfaces;

public interface IAnswerService
{
    Task AddAnswerAsync(int questionId, string userEmail, string content);
}
