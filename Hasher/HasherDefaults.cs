namespace SimpleHasher.Internals;

public static class HasherDefaults
{
    /// <summary>
    /// When enabled, the default hasher will try to recursively calculate hashes instead of just top level ones. Default is disabled
    /// </summary>
    public static bool CalculatedNestedHashes { get; set; } = false;

    /// <summary>
    /// When enabled, the default hasher will try to iterate enumerables to calculate a hash instead of just using the enumerables hash. Default is enabled.
    /// </summary>
    public static bool IterateEnumerables { get; set; } = true;
}
