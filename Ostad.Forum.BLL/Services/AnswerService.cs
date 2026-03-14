using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Services;

public class AnswerService(IUnitOfWork unitOfWork) : IAnswerService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task AddAnswerAsync(int questionId, string userEmail, string content)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email is required.", nameof(userEmail));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

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

        var answer = new Answer
        {
            QuestionId = questionId,
            UserId = user.UserId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Answers.AddAsync(answer);
        await _unitOfWork.SaveChangesAsync();
    }
}
