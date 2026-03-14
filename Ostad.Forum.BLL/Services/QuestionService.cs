using Microsoft.EntityFrameworkCore;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.BLL.Services;

public class QuestionService(IUnitOfWork unitOfWork) : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Question?> GetByIdAsync(int questionId)
    {
        var query = _unitOfWork.Questions
            .Query()
            .Include(q => q.Category)
            .Include(q => q.User)
            .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
            .Include(q => q.Answers).ThenInclude(a => a.User);

        var question = await query.FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question != null)
        {
            question.ViewCount++;
            _unitOfWork.Questions.Update(question);
            await _unitOfWork.SaveChangesAsync();
        }

        return question;
    }

    public async Task<IReadOnlyList<Question>> GetLatestQuestionsAsync(int take = 20)
    {
        var queryable = _unitOfWork.Questions
            .Query()
            .Include(q => q.Category)
            .Include(q => q.Answers);

        return await queryable
            .OrderByDescending(q => q.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    private static string ToSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        text = text.Trim().ToLowerInvariant();
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var slug = string.Join("-", parts);
        var allowed = new char[slug.Length];
        int j = 0;
        for (int i = 0; i < slug.Length; i++)
        {
            var c = slug[i];
            if (char.IsLetterOrDigit(c) || c == '-') allowed[j++] = c;
        }
        return new string(allowed, 0, j).Trim('-') switch { { Length: 0 } s => "tag", var s => s };
    }

    public async Task CreateQuestionAsync(string userEmail, string title, string description, int categoryId, IEnumerable<int> tagIds, IEnumerable<string> newTagNames)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new ArgumentException("User email is required.", nameof(userEmail));
        }

        var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email == userEmail);
        var user = existingUsers.FirstOrDefault();

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

        var question = new Question
        {
            Title = title,
            Description = description,
            CategoryId = categoryId,
            UserId = user.UserId,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();

        var distinctTagIds = tagIds.Distinct().ToList();

        var cleanedNewNames = newTagNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (cleanedNewNames.Count > 0)
        {
            var allTags = await _unitOfWork.Tags.GetAllAsync();
            foreach (var name in cleanedNewNames)
            {
                var existing = allTags.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    distinctTagIds.Add(existing.TagId);
                    continue;
                }

                var slug = ToSlug(name);
                var slugExists = allTags.Any(t => string.Equals(t.Slug, slug, StringComparison.OrdinalIgnoreCase));
                var finalSlug = slugExists ? slug + "-" + DateTime.UtcNow.Ticks : slug;

                var newTag = new Tag
                {
                    Name = name,
                    Slug = finalSlug,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Tags.AddAsync(newTag);
                await _unitOfWork.SaveChangesAsync();
                allTags = await _unitOfWork.Tags.GetAllAsync();
                distinctTagIds.Add(newTag.TagId);
            }
        }

        distinctTagIds = distinctTagIds.Distinct().ToList();

        foreach (var tagId in distinctTagIds)
        {
            await _unitOfWork.QuestionTags.AddAsync(new QuestionTag
            {
                QuestionId = question.QuestionId,
                TagId = tagId
            });
        }

        await _unitOfWork.SaveChangesAsync();
    }
}

