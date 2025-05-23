namespace SipSavy.Web.Features;

internal interface IHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}