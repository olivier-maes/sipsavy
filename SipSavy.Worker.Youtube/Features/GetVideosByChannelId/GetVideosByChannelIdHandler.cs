using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using SipSavy.Core;

namespace SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

public sealed class GetVideosByChannelIdHandler(IConfiguration configuration)
    : IHandler<GetVideosByChannelIdRequest, GetVideosByChannelIdResponse>
{
    public async Task<GetVideosByChannelIdResponse> Handle(GetVideosByChannelIdRequest request)
    {
        var apiKey = configuration["YOUTUBE_API_KEY"] ??
                     throw new InvalidOperationException("YOUTUBE_API_KEY is required.");

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "sipsavy-worker"
        });

        var channelRequest = youtubeService.Channels.List("contentDetails");
        channelRequest.Id = request.ChannelId;
        var channelResponse = await channelRequest.ExecuteAsync();
        var uploadsId = channelResponse.Items.First().ContentDetails.RelatedPlaylists.Uploads;

        var videos = new List<PlaylistItem>();
        string? nextPageToken = null;
        do
        {
            var playlistRequest = youtubeService.PlaylistItems.List("snippet,contentDetails");
            playlistRequest.PlaylistId = uploadsId;
            playlistRequest.MaxResults = 50;
            playlistRequest.PageToken = nextPageToken;

            var playlistResponse = await playlistRequest.ExecuteAsync();
            videos.AddRange(playlistResponse.Items);

            nextPageToken = playlistResponse.NextPageToken;
        } while (nextPageToken is not null);

        return new GetVideosByChannelIdResponse
        {
            Videos = videos.Select(x => new GetVideosByChannelIdResponse.VideoDto
            {
                Id = x.Id,
                Title = x.Snippet.Title,
            }).ToList()
        };
    }
}