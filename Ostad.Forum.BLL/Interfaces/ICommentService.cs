using Ostad.Forum.Contract;

namespace Ostad.Forum.BLL.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(int answerId, string userEmail, string content, int? parentCommentId = null);
    Task<IReadOnlyList<CommentTreeDto>> GetCommentsTreeForAnswerAsync(int answerId);
}
