namespace SurveyBasket.Api.Interfaces;

public interface IDistributedCashService
{
    Task<T?> GetAsunc<T>(string key, CancellationToken cancellationToken = default(CancellationToken)) where T : class;
    Task SetAsunc<T>(string key, T value, CancellationToken cancellationToken = default(CancellationToken)) where T : class;
    Task RemoveAsunc(string key, CancellationToken cancellationToken = default(CancellationToken));

}
