using Azure.AI.Inference;
using Microsoft.Extensions.Options;

namespace DevCopilot.Api.AI.Services
{
    public class ChatService
    {

        private readonly ChatCompletionsClient _chat;
        private readonly string _model;
        public ChatService(ChatCompletionsClient chat, IOptions<GitHubModelsOptions> opts)
        {
            _chat = chat;
            _model = opts.Value.ChatModel;
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string? system = null)
        {
            var options = new ChatCompletionsOptions
            {
                // For GitHub Models you specify the model in the request
                Model = _model,
                Messages =
                {
                    new ChatRequestSystemMessage(string.IsNullOrWhiteSpace(system)
                    ? "You are a concise senior .NET architect."
                    : system!),
                    new ChatRequestUserMessage(userMessage)
                }
            };

            var response = await _chat.CompleteAsync(options);
            return response.Value.Content; // aggregated assistant content
        }

        public async IAsyncEnumerable<string> StreamChatAsync(string userMessage, string? system = null)
        {
            var options = new ChatCompletionsOptions
            {
                Model = _model,
                Messages =
                {
                    new ChatRequestSystemMessage(string.IsNullOrWhiteSpace(system)
                    ? "You are a concise senior .NET architect."
                    : system!),
                    new ChatRequestUserMessage(userMessage)
                }
            };

            var stream = await _chat.CompleteStreamingAsync(options);
            await foreach (var update in stream)
            {
                if (!string.IsNullOrEmpty(update.ContentUpdate))
                    yield return update.ContentUpdate!;
            }
        }

    }
}
