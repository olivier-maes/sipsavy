namespace SipSavy.Worker.Youtube.Features.ExtractTranscription;

public sealed record ExtractTranscriptionResponse
{
    public List<TranscriptEntry> TranscriptEntries { get; init; } = [];

    public sealed record TranscriptEntry
    {
        public double Start { get; set; }
        public double Duration { get; set; }
        public string Text { get; set; } = string.Empty;

        public TimeSpan StartTime => TimeSpan.FromSeconds(Start);
        public TimeSpan EndTime => TimeSpan.FromSeconds(Start + Duration);
    };
};