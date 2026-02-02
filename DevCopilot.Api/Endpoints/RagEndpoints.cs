using DevCopilot.Api.AI.Services;
using DevCopilot.Api.Models;

namespace DevCopilot.Api.Endpoints
{
    public static class RagEndpoints
    {
        public static IEndpointRouteBuilder MapRagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/ask", async (AskRequest req, RagService rag) =>
            {
                var answer = await rag.AnswerAsync(req.Question);
                return Results.Ok(new { answer });
            });

            return app;
        }

    }
}
