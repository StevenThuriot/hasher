namespace SimpleHasher;

public interface IHashOptions
{
    /// <summary>
    /// Manually configure how to calculate the HashCode for the set type
    /// </summary>
    /// <typeparam name="T">The type the mapping is for</typeparam>
    /// <param name="values">The values to use for the HashCode</param>
    /// <exception cref="ArgumentNullException">When <paramref name="values"/> is null or empty</exception>
    IHashOptions Register<T>(params Func<T, object?>[] values);

    /// <summary>
    /// Manually configure how to asynchrously calculate the HashCode for the set type
    /// </summary>
    /// <typeparam name="T">The type the mapping is for</typeparam>
    /// <param name="values">The values to use for the HashCode</param>
    /// <exception cref="ArgumentNullException">When <paramref name="values"/> is null or empty</exception>
    IHashOptions RegisterAsync<T>(params Func<T, Task<object?>>[] values);

    /// <summary>
    /// When enabled, the default hasher will try to recursively calculate hashes instead of just top level ones.
    /// </summary>
    IHashOptions EnableNestedHashes();

    /// <summary>
    /// When enabled, the default hasher will try to recursively calculate hashes instead of just top level ones.
    /// </summary>
    IHashOptions DisableNestedHashes();

    /// <summary>
    /// When enabled, the default hasher will try to iterate enumerables to calculate a hash instead of just using the enumerables hash.
    /// </summary>
    IHashOptions EnableIterateEnumerables();

    /// <summary>
    /// When enabled, the default hasher will try to iterate enumerables to calculate a hash instead of just using the enumerables hash.
    /// </summary>
    IHashOptions DisableIterateEnumerables();
}

internal interface IHashOptionsInternal
{
    bool CalculatedNestedHashes { get; }
    bool IterateEnumerables { get; }
}
