using Microsoft.EntityFrameworkCore;
using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data;

public class VideoRepository(WorkerDbContext dbContext) : IVideoRepository
{
    public async Task<Video> AddVideo(Video video)
    {
        await dbContext.Videos.AddAsync(video);
        await dbContext.SaveChangesAsync();
        return video;
    }

    public async Task<Video?> UpdateVideo(int id, string transcription, Status status)
    {
        var existingVideo = await dbContext.Videos.FirstOrDefaultAsync(x => x.Id == id);
        if (existingVideo is null)
        {
            return null;
        }

        existingVideo.Transcription = transcription;
        existingVideo.Status = status;

        await dbContext.SaveChangesAsync();

        return existingVideo;
    }
}