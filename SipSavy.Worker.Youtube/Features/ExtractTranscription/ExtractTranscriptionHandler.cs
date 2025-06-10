using System.Text.Json;
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

        var transcriptXml = await _httpClient.GetStringAsync(transcriptUrls[0]);

        return new ExtractTranscriptionResponse
        {
            TranscriptEntries = ParseTranscriptXml(transcriptXml),
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
            var captions = JsonSerializer.Deserialize<List<CaptionTrack>>(captionData);

            urls.AddRange(from caption in captions
                where !string.IsNullOrEmpty(caption.BaseUrl)
                select caption.BaseUrl);
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

        return textCodes.Cast<XmlNode>().Select(node => new ExtractTranscriptionResponse.TranscriptEntry
        {
            Start = double.Parse(node.Attributes["start"]?.Value ?? "0"),
            Duration = double.Parse(node.Attributes["dur"]?.Value ?? "0"),
            Text = HttpUtility.HtmlDecode(node.InnerText?.Trim() ?? "")
        }).Where(entry => !string.IsNullOrEmpty(entry.Text)).ToList();
    }

    private sealed record CaptionTrack(string BaseUrl, string LanguageCode, string Name);
}