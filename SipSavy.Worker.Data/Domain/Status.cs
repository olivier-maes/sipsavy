namespace SipSavy.Worker.Data.Domain;

public enum Status
{
    New = 0,
    TranscriptionFetched = 1,
    Embedded = 2,
    Processed = 3,
}