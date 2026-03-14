namespace Ostad.Forum.Contract;

public class CreateQuestionDto
{
    public List<SelectOptionDto> Categories { get; set; } = new();
    public List<SelectOptionDto> Tags { get; set; } = new();
}
