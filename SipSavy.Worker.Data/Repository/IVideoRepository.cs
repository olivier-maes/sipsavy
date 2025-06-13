using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data.Repository;

public interface IVideoRepository
{
    Task<Video> AddVideo(Video video);
    Task<Video?> UpdateVideo(int id, string? transcription, Status? status);
}