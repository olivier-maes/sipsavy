using Mediator;

namespace SipSavy.Worker.Features.Youtube.ExtractTranscription;

public sealed record ExtractTranscriptionRequest(string YoutubeVideoId) : IRequest<ExtractTranscriptionResponse>;