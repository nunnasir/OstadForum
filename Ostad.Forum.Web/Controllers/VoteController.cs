using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ostad.Forum.BLL.Interfaces;

namespace Ostad.Forum.Web.Controllers;

[Authorize]
public class VoteController(IVoteService voteService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Question(int questionId, bool up, int? returnQuestionId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        await voteService.VoteQuestionAsync(questionId, email, up);
        return RedirectToAction("Details", "Question", new { id = returnQuestionId ?? questionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer(int answerId, bool up, int questionId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        await voteService.VoteAnswerAsync(answerId, email, up);
        return RedirectToAction("Details", "Question", new { id = questionId });
    }
}
