using System.Diagnostics.CodeAnalysis;

namespace SimpleHasher.Internals;

internal class HashOptions : IHashOptions
{
    private readonly Dictionary<Type, ISynchronousHasher> _hashers = new();
    private readonly Dictionary<Type, IAsynchronousHasher> _asyncHashers = new();

    public bool CalculatedNestedHashes { get; set; } = false;

    internal bool TryGet<T>([NotNullWhen(true)] out ISynchronousHasher<T>? hasher)
    {
        if (_hashers.TryGetValue(typeof(T), out ISynchronousHasher? boxedHasher) && boxedHasher is ISynchronousHasher<T> typedHasher)
        {
            hasher = typedHasher;
            return true;
        }

        hasher = default;
        return false;
    }

    internal bool TryGetAsync<T>([NotNullWhen(true)] out IAsynchronousHasher<T>? hasher)
    {
        if (_asyncHashers.TryGetValue(typeof(T), out IAsynchronousHasher? boxedHasher) && boxedHasher is IAsynchronousHasher<T> typedHasher)
        {
            hasher = typedHasher;
            return true;
        }

        hasher = default;
        return false;
    }

    public IHashOptions EnableNestedHashes()
    {
        CalculatedNestedHashes = true;
        return this;
    }

    public IHashOptions Register<T>(params Func<T, object?>[] values)
    {
        if (values is null || values.Length == 0)
        {
            throw new ArgumentNullException(nameof(values));
        }

        _hashers[typeof(T)] = new SynchronousHasher<T>(this, values);
        return this;
    }

    public IHashOptions RegisterAsync<T>(params Func<T, Task<object?>>[] values)
    {
        if (values is null || values.Length == 0)
        {
            throw new ArgumentNullException(nameof(values));
        }

        _asyncHashers[typeof(T)] = new AsynchronousHasher<T>(this, values);
        return this;
    }
}
