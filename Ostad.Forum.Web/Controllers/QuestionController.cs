using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.DAL.Interfaces;
using Ostad.Forum.Domain.Entities;
using Ostad.Forum.Web.Models;

namespace Ostad.Forum.Web.Controllers;

public class QuestionController(IQuestionService questionService, IAnswerService answerService, IUnitOfWork unitOfWork) : Controller
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IAnswerService _answerService = answerService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question == null)
            return NotFound();

        var model = new QuestionDetailsViewModel
        {
            QuestionId = question.QuestionId,
            Title = question.Title,
            Description = question.Description,
            CategoryName = question.Category?.Name ?? "",
            AuthorName = question.User?.Email ?? "",
            CreatedAt = question.CreatedAt,
            ViewCount = question.ViewCount,
            TagNames = question.QuestionTags?.Select(qt => qt.Tag.Name).ToList() ?? new List<string>(),
            Answers = (question.Answers ?? new List<Answer>())
                .OrderBy(a => a.CreatedAt)
                .Select(a => new AnswerViewModel
                {
                    AnswerId = a.AnswerId,
                    Content = a.Content,
                    AuthorName = a.User?.Email ?? "",
                    CreatedAt = a.CreatedAt
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
        var model = new QuestionCreateViewModel
        {
            Categories = (await _unitOfWork.Categories.GetAllAsync())
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name }),
            Tags = (await _unitOfWork.Tags.GetAllAsync())
                .Select(t => new SelectListItem { Value = t.TagId.ToString(), Text = t.Name })
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
            model.Categories = (await _unitOfWork.Categories.GetAllAsync())
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name });
            model.Tags = (await _unitOfWork.Tags.GetAllAsync())
                .Select(t => new SelectListItem { Value = t.TagId.ToString(), Text = t.Name });
            return View(model);
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return Challenge();
        }

        var newTagNames = (model.NewTags ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());

        await _questionService.CreateQuestionAsync(
            email,
            model.Title,
            model.Description,
            model.CategoryId,
            model.SelectedTagIds,
            newTagNames);

        return RedirectToAction("Index", "Home");
    }
}

