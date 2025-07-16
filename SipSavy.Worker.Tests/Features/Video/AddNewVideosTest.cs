using SipSavy.Data;
using SipSavy.Data.Repository;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Tests.Fixtures;
using SipSavy.Worker.Tests.TestData;

namespace SipSavy.Worker.Tests.Features.Video;

public class AddNewVideosTest(PostgresFixture fixture) : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldAddNewVideo_WhenVideoDoesNotExist()
    {
        // Arrange
        var queryFacade = new QueryFacade(fixture.DbContext);
        var repo = new VideoRepository(fixture.DbContext);

        var request = new AddNewVideosRequest
        {
            Videos =
            [
                new AddNewVideosRequest.VideoDto
                    { VideoId = TestVideo.Video1.YoutubeId, Title = TestVideo.Video1.Title },
                new AddNewVideosRequest.VideoDto { VideoId = "video2", Title = "Video 2" }
            ]
        };

        // Act
        var sut = new AddNewVideosHandler(queryFacade, repo);
        var response = await sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsType<AddNewVideosResponse>(response);
        Assert.Single(response.Videos);
        Assert.Equal(request.Videos.Find(x => x.VideoId == "video2")?.Title, response.Videos.First().Title);
    }

    public Task InitializeAsync()
    {
        return fixture.ClearAndReseedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}