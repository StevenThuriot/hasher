namespace SimpleHasher.Internals;

internal interface IAsynchronousHasher
{
    Task<int> Get(object? value, HashService service);
}

internal interface IAsynchronousHasher<T> : IAsynchronousHasher
{
    Task<int> Get(T? value, HashService service);
}

internal sealed class AsynchronousHasher<T> : IAsynchronousHasher<T>
{
    private readonly IHashOptionsInternal _options;
    private readonly IEnumerable<Func<T, Task<object?>>> _getters;

    public AsynchronousHasher(IHashOptionsInternal options, IEnumerable<Func<T, Task<object?>>> getters)
    {
        _options = options;
        _getters = getters;
    }

    public Task<int> Get(T? value, HashService service)
    {
        if (value is null)
        {
            return Task.FromResult(0);
        }

        return GetInternal(value, service, _options, _getters);
    }

    Task<int> IAsynchronousHasher.Get(object? value, HashService service)
    {
        if (value is null)
        {
            return Task.FromResult(0);
        }

        if (value is T typedValue)
        {
            return GetInternal(typedValue, service, _options, _getters);
        }

        return Task.FromException<int>(new ArgumentException("Wrong type", nameof(value)));
    }

    internal static async Task<int> GetInternal(T value, HashService service, IHashOptionsInternal options, IEnumerable<Func<T, Task<object?>>> getters)
    {
        SimpleHashCode hashCode = new();

        foreach (Func<T, Task<object?>> get in getters)
        {
            object? propertyValue = await get(value).ConfigureAwait(false);

            if (propertyValue is null)
            {
                hashCode.Add(0);
            }
            else if (!options.CalculatedNestedHashes)
            {
                hashCode.Add(propertyValue);
            }
            else
            {
                int nestedHash = await NestedHashHelper.ResolveNestedHashAsync(propertyValue, service).ConfigureAwait(false);
                hashCode.Add(nestedHash);
            }
        }

        return hashCode.ToHashCode();
    }
}
