namespace SipSavy.Worker.Data.Domain;

public sealed class Video
{
    public int Id { get; set; }
    public string YoutubeId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Transcription { get; set; } = string.Empty;

    public Status Status { get; set; } = Status.New;

    public List<VideoChunk> Chunks { get; set; } = [];
}