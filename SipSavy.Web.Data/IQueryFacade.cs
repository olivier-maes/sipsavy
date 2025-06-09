using SipSavy.Web.Data.Domain;

namespace SipSavy.Web.Data;

public interface IQueryFacade
{
    IQueryable<Cocktail> Cocktails { get; }
}