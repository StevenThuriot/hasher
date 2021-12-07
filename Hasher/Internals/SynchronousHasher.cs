namespace SimpleHasher.Internals;

internal interface ISynchronousHasher
{
    int Get(object? value, HashService service);
}

internal interface ISynchronousHasher<T> : ISynchronousHasher
{
    int Get(T? value, HashService service);
}

internal sealed class SynchronousHasher<T> : ISynchronousHasher<T>
{
    private readonly IHashOptionsInternal _options;
    private readonly IEnumerable<Func<T, object?>> _getters;

    public SynchronousHasher(IHashOptionsInternal options, IEnumerable<Func<T, object?>> getters)
    {
        _options = options;
        _getters = getters;
    }

    public int Get(T? value, HashService service)
    {
        if (value is null)
        {
            return 0;
        }

        return GetInternal(value, service, _options, _getters);
    }

    int ISynchronousHasher.Get(object? value, HashService service)
    {
        if (value is null)
        {
            return 0;
        }

        if (value is T typedValue)
        {
            return GetInternal(typedValue, service, _options, _getters);
        }

        throw new ArgumentException("Wrong type", nameof(value));
    }

    internal static int GetInternal(T value, HashService service, IHashOptionsInternal options, IEnumerable<Func<T, object?>> getters)
    {
        HashCode hashCode = new();

        foreach (Func<T, object?> get in getters)
        {
            object? propertyValue = get(value);

            if (options.CalculatedNestedHashes && propertyValue is not null)
            {
                propertyValue = NestedHashHelper.ResolveNestedHash(propertyValue, service);
            }

            hashCode.Add(propertyValue);
        }

        return hashCode.ToHashCode();
    }
}
