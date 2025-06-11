using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Features.Video.UpdateVideo;

internal sealed record UpdateVideoRequest(int Id, string Transcription, Status Status);