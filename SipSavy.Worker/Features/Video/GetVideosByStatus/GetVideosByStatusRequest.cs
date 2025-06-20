using SipSavy.Data.Domain;

namespace SipSavy.Worker.Features.Video.GetVideosByStatus;

internal sealed record GetVideosByStatusRequest(Status Status);