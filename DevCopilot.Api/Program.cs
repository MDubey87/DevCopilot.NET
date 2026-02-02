using Azure;
using Azure.AI.Inference;
using DevCopilot.Api;
using DevCopilot.Api.AI.Services;
using DevCopilot.Api.Endpoints;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<GitHubModelsOptions>(cfg.GetSection("GitHubModels"));

// Register clients against GitHub Models endpoint using PAT as AzureKeyCredential
builder.Services.AddSingleton(sp =>
{
    var opt = sp.GetRequiredService<IOptions<GitHubModelsOptions>>().Value;
    var token = builder.Configuration["GitHubModels:Token"]
                ?? throw new InvalidOperationException("GitHubModels:Token is missing (user-secrets).");
    return new ChatCompletionsClient(new Uri(opt.Endpoint), new AzureKeyCredential(token));
});

builder.Services.AddSingleton(sp =>
{
    var opt = sp.GetRequiredService<IOptions<GitHubModelsOptions>>().Value;
    var token = builder.Configuration["GitHubModels:Token"]
                ?? throw new InvalidOperationException("GitHubModels:Token is missing (user-secrets).");
    return new EmbeddingsClient(new Uri(opt.Endpoint), new AzureKeyCredential(token));
});

builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<RagService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapChatEndpoints();
app.MapRagEndpoints();


app.Run();

