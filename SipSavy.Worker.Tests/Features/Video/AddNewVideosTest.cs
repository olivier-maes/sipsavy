using SipSavy.Worker.Data;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Tests.Fixtures;
using SipSavy.Worker.Tests.TestData;

namespace SipSavy.Worker.Tests.Features.Video;

public class AddNewVideosTest : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Handle_ShouldAddNewVideo_WhenVideoDoesNotExist()
    {
        // Arrange
        var dbContext = await PostgresFixture.GetDbContext();
        var queryFacade = new QueryFacade(dbContext);
        var repo = new VideoRepository(dbContext);

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
        var response = await sut.Handle(request);

        // Assert
        Assert.IsType<AddNewVideosResponse>(response);
        Assert.Single(response.Videos);
        Assert.Equal(request.Videos.Find(x => x.VideoId == "video2")?.Title, response.Videos.First().Title);

        // Cleanup
        var video = dbContext.Videos.Single(x => x.YoutubeId == "video2");
        dbContext.Videos.Remove(video);
        await dbContext.SaveChangesAsync();
    }
}