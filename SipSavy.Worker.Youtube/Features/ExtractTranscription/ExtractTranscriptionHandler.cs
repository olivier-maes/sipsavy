using System.Text.Json;
using System.Text.Json.Serialization;
using SipSavy.Core;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace SipSavy.Worker.Youtube.Features.ExtractTranscription;

public sealed class ExtractTranscriptionHandler : IHandler<ExtractTranscriptionRequest, ExtractTranscriptionResponse>
{
    private readonly HttpClient _httpClient;

    public ExtractTranscriptionHandler()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task<ExtractTranscriptionResponse> Handle(ExtractTranscriptionRequest request)
    {
        var html = await _httpClient.GetStringAsync($"https://www.youtube.com/watch?v={request.YoutubeVideoId}");

        var transcriptUrls = ExtractTranscriptUrls(html);
        if (transcriptUrls.Count == 0)
        {
            return new ExtractTranscriptionResponse();
        }

        var transcriptRequest = new HttpRequestMessage(HttpMethod.Get, transcriptUrls[0]);
        transcriptRequest.Headers.Add("Accept",
            "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");

        var transcriptResponse = await _httpClient.SendAsync(transcriptRequest);
        if (transcriptResponse.IsSuccessStatusCode)
        {
            var transcriptXml = await transcriptResponse.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(transcriptXml) && transcriptXml.Contains("<text"))
            {
                Console.WriteLine($"Successfully got transcript: {transcriptXml.Length} characters");
                var parsed = ParseTranscriptXml(transcriptXml);
            }
            else
            {
                Console.WriteLine($"Empty or invalid transcript response: {transcriptXml}");
            }
        }

        return new ExtractTranscriptionResponse
        {
            TranscriptEntries = [],
        };
    }

    private static List<string> ExtractTranscriptUrls(string html)
    {
        var urls = new List<string>();

        const string pattern = @"""captionTracks"":\[(.*?)\]";
        var match = Regex.Match(html, pattern);

        if (!match.Success) return urls;

        try
        {
            var captionData = "[" + match.Groups[1].Value + "]";
            var captions = JsonSerializer.Deserialize<List<CaptionTrack>>(captionData) ?? [];

            urls.AddRange(captions.Select(x => x.BaseUrl));
        }
        catch
        {
            const string urlPattern = @"""baseUrl"":""([^""]+)""";
            var urlMatches = Regex.Matches(html, urlPattern);

            foreach (Match urlMatch in urlMatches)
            {
                var url = urlMatch.Groups[1].Value;
                if (url.Contains("timedtext"))
                {
                    urls.Add(HttpUtility.HtmlDecode(url));
                }
            }
        }

        return urls;
    }

    private static List<ExtractTranscriptionResponse.TranscriptEntry> ParseTranscriptXml(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var textCodes = doc.SelectNodes("//text");

        return textCodes?.Cast<XmlNode>().Select(node => new ExtractTranscriptionResponse.TranscriptEntry
        {
            Start = double.Parse(node?.Attributes?["start"]?.Value ?? "0"),
            Duration = double.Parse(node?.Attributes?["dur"]?.Value ?? "0"),
            Text = HttpUtility.HtmlDecode(node?.InnerText.Trim() ?? "")
        }).Where(entry => !string.IsNullOrEmpty(entry.Text)).ToList() ?? [];
    }

    private class CaptionTrack
    {
        [JsonPropertyName("baseUrl")] public string BaseUrl { get; set; } = string.Empty;
        [JsonPropertyName("name")] public CaptionTrackName Name { get; set; } = new();
        [JsonPropertyName("vssId")] public string VssId { get; set; } = string.Empty;
        [JsonPropertyName("languageCode")] public string LanguageCode { get; set; } = string.Empty;
        [JsonPropertyName("kind")] public string Kind { get; set; } = string.Empty;
        [JsonPropertyName("isTranslatable")] public bool IsTranslatable { get; set; }
        [JsonPropertyName("trackName")] public string TrackName { get; set; } = string.Empty;
    }

    private class CaptionTrackName
    {
        [JsonPropertyName("simpleText")] public string SimpleText { get; set; } = string.Empty;
    }
}