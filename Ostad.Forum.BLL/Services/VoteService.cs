using Microsoft.EntityFrameworkCore;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Services;

public class VoteService(IUnitOfWork unitOfWork) : IVoteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<int> GetQuestionScoreAsync(int questionId)
    {
        var votes = await _unitOfWork.Votes
            .Query()
            .Where(v => v.QuestionId == questionId)
            .ToListAsync();
        return votes.Sum(v => (int)v.VoteType);
    }

    public async Task<int> GetAnswerScoreAsync(int answerId)
    {
        var votes = await _unitOfWork.Votes
            .Query()
            .Where(v => v.AnswerId == answerId)
            .ToListAsync();
        return votes.Sum(v => (int)v.VoteType);
    }

    public async Task VoteQuestionAsync(int questionId, string userEmail, bool isUpvote)
    {
        var user = await ResolveUserAsync(userEmail);
        var voteType = isUpvote ? VoteType.Upvote : VoteType.Downvote;

        var existing = await _unitOfWork.Votes
            .Query()
            .FirstOrDefaultAsync(v => v.QuestionId == questionId && v.UserId == user.UserId);

        if (existing != null)
        {
            if (existing.VoteType == voteType)
            {
                _unitOfWork.Votes.Remove(existing);
            }
            else
            {
                existing.VoteType = voteType;
                _unitOfWork.Votes.Update(existing);
            }
        }
        else
        {
            await _unitOfWork.Votes.AddAsync(new Vote
            {
                QuestionId = questionId,
                UserId = user.UserId,
                VoteType = voteType,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task VoteAnswerAsync(int answerId, string userEmail, bool isUpvote)
    {
        var user = await ResolveUserAsync(userEmail);
        var voteType = isUpvote ? VoteType.Upvote : VoteType.Downvote;

        var existing = await _unitOfWork.Votes
            .Query()
            .FirstOrDefaultAsync(v => v.AnswerId == answerId && v.UserId == user.UserId);

        if (existing != null)
        {
            if (existing.VoteType == voteType)
            {
                _unitOfWork.Votes.Remove(existing);
            }
            else
            {
                existing.VoteType = voteType;
                _unitOfWork.Votes.Update(existing);
            }
        }
        else
        {
            await _unitOfWork.Votes.AddAsync(new Vote
            {
                AnswerId = answerId,
                UserId = user.UserId,
                VoteType = voteType,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();
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
