using SipSavy.Data.Domain;

namespace SipSavy.Data.Repository;

public interface IVideoRepository
{
    Task<Video> AddVideo(Video video);
    Task<Video?> UpdateVideo(int id, string? transcription, Status? status);
}