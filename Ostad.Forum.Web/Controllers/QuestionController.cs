using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Web.Models;

namespace Ostad.Forum.Web.Controllers;

public class QuestionController(
    IQuestionService questionService,
    IAnswerService answerService,
    IVoteService voteService) : Controller
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IAnswerService _answerService = answerService;
    private readonly IVoteService _voteService = voteService;

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _questionService.GetQuestionDetailsAsync(id);
        if (dto == null)
            return NotFound();

        var model = new QuestionDetailsViewModel
        {
            QuestionId = dto.QuestionId,
            Title = dto.Title,
            Description = dto.Description,
            CategoryName = dto.CategoryName,
            AuthorName = dto.AuthorName,
            CreatedAt = dto.CreatedAt,
            ViewCount = dto.ViewCount,
            QuestionScore = dto.QuestionScore,
            TagNames = dto.TagNames,
            Answers = dto.Answers.Select(a => new AnswerViewModel
            {
                AnswerId = a.AnswerId,
                Content = a.Content,
                AuthorName = a.AuthorName,
                CreatedAt = a.CreatedAt,
                Score = a.Score
            }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddAnswer(AnswerInputModel model)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        if (string.IsNullOrWhiteSpace(model?.Content))
        {
            TempData["AnswerError"] = "Answer content is required.";
            return RedirectToAction(nameof(Details), new { id = model!.QuestionId });
        }

        await _answerService.AddAnswerAsync(model.QuestionId, email, model.Content);
        return RedirectToAction(nameof(Details), new { id = model.QuestionId });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var dto = await _questionService.GetCreatePageDataAsync();
        var model = new QuestionCreateViewModel
        {
            Categories = dto.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text }),
            Tags = dto.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(QuestionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var dto = await _questionService.GetCreatePageDataAsync();
            model.Categories = dto.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text });
            model.Tags = dto.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text });
            return View(model);
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        var newTagNames = (model.NewTags ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());

        await _questionService.CreateQuestionAsync(
            email,
            model.Title,
            model.Description,
            model.CategoryId,
            model.SelectedTagIds ?? new List<int>(),
            newTagNames);

        return RedirectToAction("Index", "Home");
    }
}
