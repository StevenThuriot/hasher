using SimpleHasher.Internals;

namespace SimpleHasher.Extensions;

public static class HashExtensions
{
    private static readonly HashOptions s_options = new();

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static int ResolveHash<T>(this T value)
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(Task).IsAssignableFrom(typeof(T)))
        {
            return value?.GetHashCode() ?? 0;
        }

        return DefaultHasher<T>.Get(value, null!, s_options);
    }

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="propertyNames">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static int ResolveHash<T>(this T value, params string[] propertyNames)
    {
        if (propertyNames is null || propertyNames.Length == 0)
        {
            throw new ArgumentNullException(nameof(propertyNames));
        }

        return DefaultHasher<T>.Get(value, null!, s_options, propertyNames);
    }

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="properties"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="properties">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static int ResolveHash<T>(this T value, params Func<T, object?>[] properties)
    {
        if (properties is null || properties.Length == 0)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        return new SynchronousHasher<T>(s_options, properties).Get(value, null!);
    }

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <remarks>
    /// Same behavior as <see cref="ResolveHash{T}"/> but will check if any properties are of type <see cref="Task{T}"/> and await their values instead of getting a hashcode for the task instance.
    /// </remarks>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static Task<int> ResolveHashAsync<T>(this T value)
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return Task.FromResult(value?.GetHashCode() ?? 0);
        }

        return DefaultHasher<T>.GetAsync(value, null!, s_options);
    }

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <remarks>
    /// Same behavior as <see cref="ResolveHash{T}"/> but will check if any properties are of type <see cref="Task{T}"/> and await their values instead of getting a hashcode for the task instance.
    /// </remarks>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="propertyNames"></param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static Task<int> ResolveHashAsync<T>(this T value, params string[] propertyNames)
    {
        if (propertyNames is null || propertyNames.Length == 0)
        {
            throw new ArgumentNullException(nameof(propertyNames));
        }

        return DefaultHasher<T>.GetAsync(value, null!, s_options, propertyNames);
    }

    /// <summary>
    /// Simple hashresolver, no nesting or anything else fancy the registered service can do.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="properties"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <remarks>
    /// Same behavior as <see cref="ResolveHash{T}"/> but will check if any properties are of type <see cref="Task{T}"/> and await their values instead of getting a hashcode for the task instance.
    /// </remarks>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="properties"></param>
    /// <returns>A hash code for this value based on its properties</returns>
    public static Task<int> ResolveHashAsync<T>(this T value, params Func<T, Task<object?>>[] properties)
    {
        if (properties is null || properties.Length == 0)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        return new AsynchronousHasher<T>(s_options, properties).Get(value, null!);
    }
}