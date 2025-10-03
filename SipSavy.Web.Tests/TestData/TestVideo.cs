namespace SipSavy.Web.Tests.TestData;

internal static class TestVideo
{
    public static readonly Video Video1 = new()
    {
        Id = 1,
        YoutubeId = "video1",
        Title = "Cocktail 1",
        Transcription = "Transcription for video 1",
        Status = Status.New,
    };
}