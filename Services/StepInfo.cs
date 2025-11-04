namespace SecurityProject.Services;

public class StepInfo
{
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string? Explanation { get; set; }

    public StepInfo(int stepNumber, string description, string input, string output, string? explanation = null)
    {
        StepNumber = stepNumber;
        Description = description;
        Input = input;
        Output = output;
        Explanation = explanation;
    }
}

