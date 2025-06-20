using Microsoft.EntityFrameworkCore;
using SipSavy.Data.Domain;

namespace SipSavy.Data.Repository;

public class VideoRepository(AppDbContext dbContext) : IVideoRepository
{
    public async Task<Video> AddVideo(Video video)
    {
        await dbContext.Videos.AddAsync(video);
        await dbContext.SaveChangesAsync();
        return video;
    }

    public async Task<Video?> UpdateVideo(int id, string? transcription, Status? status)
    {
        var existingVideo = await dbContext.Videos.FirstOrDefaultAsync(x => x.Id == id);
        if (existingVideo is null)
        {
            return null;
        }

        if (transcription is not null)
        {
            existingVideo.Transcription = transcription;
        }

        if (status is not null)
        {
            existingVideo.Status = status.Value;
        }

        await dbContext.SaveChangesAsync();

        return existingVideo;
    }
}