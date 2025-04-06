
using Microsoft.Extensions.Caching.Distributed;
using SurveyBasket.Api.Interfaces;
using System.Text.Json;

namespace SurveyBasket.Api.Services;

public class DistributedCashService(IDistributedCache distributedCache) : IDistributedCashService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsunc<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cashValue = await _distributedCache.GetStringAsync(key, cancellationToken);

        return string.IsNullOrEmpty(cashValue)
            ? null 
            : JsonSerializer.Deserialize<T>(cashValue);
    }
    public async Task SetAsunc<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
    }
    public async Task RemoveAsunc(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    
}
