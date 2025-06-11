using SipSavy.Worker.Data;
using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Tests.Fixtures;

namespace SipSavy.Worker.Tests.Features.Video;

public class GetVideosByStatusTest : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task? Handle_ShouldReturnVideosByStatus()
    {
        // Arrange
        var dbContext = await PostgresFixture.GetDbContext();
        var queryFacade = new QueryFacade(dbContext);

        // Act
        var sut = new GetVideosByStatusHandler(queryFacade);
        var response = await sut.Handle(new GetVideosByStatusRequest(Status.New));

        // Assert
        Assert.IsType<GetVideosByStatusResponse>(response);
        Assert.Single(response.Videos);
        Assert.Equal(Status.New, response.Videos.First().Status);
    }
}