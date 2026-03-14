using Microsoft.EntityFrameworkCore;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Contract;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Services;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task AddCommentAsync(int answerId, string userEmail, string content, int? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email is required.", nameof(userEmail));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        var user = await ResolveUserAsync(userEmail);

        var comment = new Comment
        {
            AnswerId = answerId,
            UserId = user.UserId,
            ParentCommentId = parentCommentId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<CommentTreeDto>> GetCommentsTreeForAnswerAsync(int answerId)
    {
        var comments = await _unitOfWork.Comments
            .Query()
            .Include(c => c.User)
            .Where(c => c.AnswerId == answerId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        var byParent = comments
            .GroupBy(c => c.ParentCommentId ?? -1)
            .ToDictionary(g => g.Key, g => g.ToList());
        return BuildTree(byParent, -1);

        static List<CommentTreeDto> BuildTree(Dictionary<int, List<Comment>> byParent, int parentKey)
        {
            if (!byParent.TryGetValue(parentKey, out var list))
                return new List<CommentTreeDto>();

            return list.Select(c => new CommentTreeDto
            {
                CommentId = c.CommentId,
                Content = c.Content,
                AuthorName = c.User?.Email ?? "",
                CreatedAt = c.CreatedAt,
                Replies = BuildTree(byParent, c.CommentId)
            }).ToList();
        }
    }

    private async Task<User> ResolveUserAsync(string userEmail)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == userEmail);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            user = new User
            {
                Name = userEmail,
                Email = userEmail,
                PasswordHash = string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
        return user;
    }
}
