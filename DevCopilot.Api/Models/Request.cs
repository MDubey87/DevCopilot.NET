namespace DevCopilot.Api.Models
{
    public record ChatRequest(string Message, string? SystemPrompt);
    public record AskRequest(string Question);
}
