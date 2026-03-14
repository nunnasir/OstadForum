using System.ComponentModel.DataAnnotations;

namespace Ostad.Forum.Web.Models;

public class AnswerInputModel
{
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Please enter your answer.")]
    [MinLength(10, ErrorMessage = "Answer must be at least 10 characters.")]
    public string Content { get; set; } = null!;
}
