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

        if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(Task).IsAssignableFrom(typeof(T)))
        {
            return value?.GetHashCode() ?? 0;
        }

        return DefaultHasher<T>.Get(value, this, _options);
    }

    public int ResolveHash<T>(T value, params string[] propertyNames)
    {
        return DefaultHasher<T>.Get(value, this, _options, propertyNames);
    }

    public int ResolveHash<T>(T value, params Func<T, object?>[] properties)
    {
        return new SynchronousHasher<T>(_options, properties).Get(value, this);
    }

    public Task<int> ResolveHashAsync<T>(T value)
    {
        if (_options.TryGetAsync(out IAsynchronousHasher<T>? hasher))
        {
            return hasher.Get(value, this);
        }

        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return Task.FromResult(value?.GetHashCode() ?? 0);
        }

        return DefaultHasher<T>.GetAsync(value, this, _options);
    }

    public Task<int> ResolveHashAsync<T>(T value, params string[] propertyNames)
    {
        return DefaultHasher<T>.GetAsync(value, this, _options, propertyNames);
    }

    public Task<int> ResolveHashAsync<T>(T value, params Func<T, Task<object?>>[] properties)
    {
        return new AsynchronousHasher<T>(_options, properties).Get(value, this);
    }
}