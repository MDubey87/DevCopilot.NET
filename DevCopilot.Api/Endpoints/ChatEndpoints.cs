using DevCopilot.Api.AI.Services;
using DevCopilot.Api.Models;

namespace DevCopilot.Api.Endpoints
{
    public static class ChatEndpoints
    {
        public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/chat", async (ChatRequest req, ChatService chat) =>
            {
                var text = await chat.GetChatResponseAsync(req.Message, req.SystemPrompt);
                return Results.Ok(new { text });
            });

            app.MapPost("/api/stream", async (ChatRequest req, ChatService chat, HttpContext ctx) =>
            {
                ctx.Response.Headers.Append("Content-Type", "text/event-stream");
                await foreach (var token in chat.StreamChatAsync(req.Message, req.SystemPrompt))
                {
                    await ctx.Response.WriteAsync($"data: {token}\n\n");
                    await ctx.Response.Body.FlushAsync();
                }
                return Results.Empty;
            });

            return app;
        }

    }
}
