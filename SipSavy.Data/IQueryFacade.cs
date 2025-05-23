using SipSavy.Data.Domain;

namespace SipSavy.Data;

public interface IQueryFacade
{
    IQueryable<Cocktail> Cocktails { get; }
}