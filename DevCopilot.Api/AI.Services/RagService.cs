using Azure.AI.Inference;
using Microsoft.Extensions.Options;

namespace DevCopilot.Api.AI.Services
{
    public class RagService
    {

        private readonly EmbeddingsClient _embeddings;
        private readonly ChatCompletionsClient _chat;
        private readonly string _embedModel;
        private readonly string _chatModel;
        private readonly List<(float[] vec, string chunk)> _index = new();

        public RagService(EmbeddingsClient embeddings, ChatCompletionsClient chat, IOptions<GitHubModelsOptions> opts, IWebHostEnvironment env)
        {
            _embeddings = embeddings;
            _chat = chat;
            _embedModel = opts.Value.EmbeddingModel;
            _chatModel = opts.Value.ChatModel;

            var kbPath = Path.Combine(env.ContentRootPath, "KnowledgeBase");
            Directory.CreateDirectory(kbPath);

            foreach (var file in Directory.EnumerateFiles(kbPath, "*.*", SearchOption.AllDirectories)
                                          .Where(f => f.EndsWith(".md") || f.EndsWith(".txt")))
            {
                var content = File.ReadAllText(file);
                foreach (var chunk in Chunk(content, approxChars: 2000, overlap: 200))
                {
                    var vec = Embed(chunk).Result; // load index on startup
                    _index.Add((vec, chunk));
                }
            }
        }

        public async Task<string> AnswerAsync(string question)
        {
            var q = await Embed(question);
            var top = _index.Select(e => new { sim = Cosine(q, e.vec), e.chunk })
                            .OrderByDescending(x => x.sim)
                            .Take(4)
                            .Select(x => x.chunk)
                            .ToList();

            var context = string.Join("\n---\n", top);
            var opts = new ChatCompletionsOptions
            {
                Model = _chatModel,
                Messages =
                {
                    new ChatRequestSystemMessage("Answer strictly from the given context. If unknown, say you don't know."),
                    new ChatRequestUserMessage($"Context:\n{context}\n\nQuestion: {question}")
                }
            };

            var response = await _chat.CompleteAsync(opts);
            return response.Value.Content;
        }

        private async Task<float[]> Embed(string text)
        {
            var req = new EmbeddingsOptions(new List<string> { text })
            {
                // For GitHub Models, set the model on the request
                Model = _embedModel
            };

            var res = await _embeddings.EmbedAsync(req);
            // Convert BinaryData -> float[]
            return res.Value.Data[0].Embedding.ToObjectFromJson<List<float>>().ToArray();
        }

        private static IEnumerable<string> Chunk(string text, int approxChars, int overlap)
        {
            for (int i = 0; i < text.Length; i += (approxChars - overlap))
                yield return text.Substring(i, Math.Min(approxChars, text.Length - i));
        }

        private static float Cosine(float[] a, float[] b)
        {
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-12));
        }

    }
}
