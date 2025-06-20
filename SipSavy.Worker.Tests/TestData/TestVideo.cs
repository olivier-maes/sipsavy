using SipSavy.Data.Domain;

namespace SipSavy.Worker.Tests.TestData;

internal sealed class TestVideo
{
    public static readonly Video Video1 = new Video
    {
        YoutubeId = "abc",
        Title = "Test Video 1",
        Transcription = "",
        Status = Status.New
    };

    public static readonly Video Video2 = new Video
    {
        YoutubeId = "def",
        Title = "Test Video 2",
        Transcription = "Sample transcription for video 2.",
        Status = Status.TranscriptionFetched
    };
}