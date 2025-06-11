using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SipSavy.Core;

namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed class GetEmbeddingsHandler : IHandler<GetEmbeddingsRequest, GetEmbeddingsResponse>
{
    private readonly HttpClient _httpClient;

    public GetEmbeddingsHandler(HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        var apiKey = configuration["OpenAI:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    public async Task<GetEmbeddingsResponse> Handle(GetEmbeddingsRequest request)
    {
        var openAiRequest = new
        {
            input = request.Texts.ToArray(),
            model = request.Model
        };

        var json = JsonSerializer.Serialize(openAiRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/embeddings", content);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OpenAiEmbeddingResponse>(responseJson);

        return new GetEmbeddingsResponse
        {
            Embedding = result?.Data?.Select(d => d.Embedding).ToList() ?? []
        };
    }

    private class OpenAiEmbeddingResponse
    {
        public List<EmbeddingData>? Data { get; set; }
    }

    private class EmbeddingData
    {
        public float[] Embedding { get; set; } = [];
    }
}