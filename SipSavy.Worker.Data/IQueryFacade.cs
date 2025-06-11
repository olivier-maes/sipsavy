using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data;

public interface IQueryFacade
{
    IQueryable <Video> Videos { get; }
}