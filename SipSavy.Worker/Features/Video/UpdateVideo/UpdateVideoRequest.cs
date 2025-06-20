using SipSavy.Data.Domain;

namespace SipSavy.Worker.Features.Video.UpdateVideo;

internal sealed record UpdateVideoRequest(int Id, string? Transcription, Status? Status);