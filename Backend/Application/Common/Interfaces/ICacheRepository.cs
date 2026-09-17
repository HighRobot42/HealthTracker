namespace Application.Common.Interfaces;

/// <summary>Generic cache-aside repository contract.</summary>
public interface ICacheRepository<T>
{
    Task<T?> GetAsync(string key, CancellationToken ct = default);
    Task SetAsync(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
