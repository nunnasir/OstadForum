using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Web.Models;

namespace Ostad.Forum.Web.Controllers;

public class HomeController(IQuestionService questionService) : Controller
{
    private readonly IQuestionService _questionService = questionService;

    public async Task<IActionResult> Index()
    {
        var questions = await _questionService.GetLatestQuestionsAsync();

        var model = new HomeIndexViewModel
        {
            Questions = questions.Select(q => new QuestionListItemViewModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                CategoryName = q.Category.Name,
                CreatedAt = q.CreatedAt,
                AnswerCount = q.Answers.Count,
                ViewCount = q.ViewCount
            }).ToList()
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
