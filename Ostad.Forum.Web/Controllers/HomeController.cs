using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Web.Models;

namespace Ostad.Forum.Web.Controllers;

public class HomeController(
    IQuestionService questionService,
    ICategoryService categoryService,
    ITagService tagService) : Controller
{
    private readonly IQuestionService _questionService = questionService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly ITagService _tagService = tagService;

    public async Task<IActionResult> Index()
    {
        // Run sequentially: all services share the same scoped DbContext (via UnitOfWork), which is not thread-safe.
        var questions = await _questionService.GetLatestQuestionsAsync();
        var categories = await _categoryService.GetAllAsync();
        var tags = await _tagService.GetAllAsync();

        var model = new HomeIndexViewModel
        {
            Questions = questions.Select(q => new QuestionListItemViewModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                CategoryName = q.CategoryName,
                CreatedAt = q.CreatedAt,
                AnswerCount = q.AnswerCount,
                ViewCount = q.ViewCount
            }).ToList(),
            Categories = categories,
            Tags = tags
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
