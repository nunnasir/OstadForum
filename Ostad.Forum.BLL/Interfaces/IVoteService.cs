namespace Ostad.Forum.BLL.Interfaces;

public interface IVoteService
{
    Task<int> GetQuestionScoreAsync(int questionId);
    Task<int> GetAnswerScoreAsync(int answerId);
    Task VoteQuestionAsync(int questionId, string userEmail, bool isUpvote);
    Task VoteAnswerAsync(int answerId, string userEmail, bool isUpvote);
}
