using SipSavy.Data;
using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Tests.Fixtures;

namespace SipSavy.Worker.Tests.Features.Video;

public class GetVideosByStatusTest(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task? Handle_ShouldReturnVideosByStatus()
    {
        // Arrange
        var queryFacade = new QueryFacade(fixture.DbContext);
        
        // Act
        var sut = new GetVideosByStatusHandler(queryFacade);
        var response = await sut.Handle(new GetVideosByStatusRequest(Status.New), CancellationToken.None);
        
        // Assert
        Assert.IsType<GetVideosByStatusResponse>(response);
        Assert.Single(response.Videos);
        Assert.Equal(Status.New, response.Videos.First().Status);
    }
}