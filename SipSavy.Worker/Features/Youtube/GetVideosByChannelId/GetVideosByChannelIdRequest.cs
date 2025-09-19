using Mediator;

namespace SipSavy.Worker.Features.Youtube.GetVideosByChannelId;

public sealed record GetVideosByChannelIdRequest(string ChannelId) : IRequest<GetVideosByChannelIdResponse>;