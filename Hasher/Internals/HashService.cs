using SimpleHasher.Extensions;

namespace SimpleHasher.Internals;

internal class HashService : IHashService
{
    private readonly HashOptions _options;

    public HashService(HashOptions options)
    {
        _options = options;
    }

    public int ResolveHash<T>(T value)
    {
        if (_options.TryGet(out ISynchronousHasher<T>? hasher))
        {
            return hasher.Get(value, this);
        }

        if (value.IsNullOrPrimitive(out int hash))
        {
            return hash;
        }

        return DefaultHasher<T>.Get(value, this, _options);
    }

    public int ResolveHash<T>(T value, params string[] propertyNames)
    {
        if (value.IsNullOrPrimitive(out int hash))
        {
            return hash;
        }

        return DefaultHasher<T>.Get(value, this, _options, propertyNames);
    }

    public int ResolveHash<T>(T value, params Func<T, object?>[] properties)
    {
        if (value.IsNullOrPrimitive(out int hash))
        {
            return hash;
        }

        return SynchronousHasher<T>.GetInternal(value, this, _options, properties);
    }

    public Task<int> ResolveHashAsync<T>(T value)
    {
        if (_options.TryGetAsync(out IAsynchronousHasher<T>? hasher))
        {
            return hasher.Get(value, this);
        }

        if (value.IsNullOrPrimitive(out int hash))
        {
            return Task.FromResult(hash);
        }

        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return Task.FromResult(value?.GetHashCode() ?? 0);
        }

        return DefaultHasher<T>.GetAsync(value, this, _options);
    }

    public Task<int> ResolveHashAsync<T>(T value, params string[] propertyNames)
    {
        if (value.IsNullOrPrimitive(out int hash))
        {
            return Task.FromResult(hash);
        }

        return DefaultHasher<T>.GetAsync(value, this, _options, propertyNames);
    }

    public Task<int> ResolveHashAsync<T>(T value, params Func<T, Task<object?>>[] properties)
    {
        if (value.IsNullOrPrimitive(out int hash))
        {
            return Task.FromResult(hash);
        }

        return AsynchronousHasher<T>.GetInternal(value, this, _options, properties);
    }
}