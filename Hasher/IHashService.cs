namespace SimpleHasher;

public interface IHashService
{
    /// <summary>
    /// HashCode Resolver service, which will either use your supplied configuration or fall back to all public properties with a getter.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <returns>A hash code for this value based on its properties</returns>
    int ResolveHash<T>(T value);

    /// <summary>
    /// HashCode Resolver service.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="propertyNames">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    int ResolveHash<T>(T value, params string[] propertyNames);

    /// <summary>
    /// HashCode Resolver service.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="properties">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    int ResolveHash<T>(T value, params Func<T, object?>[] properties);

    /// <summary>
    /// HashCode Resolver service, which will either use your supplied configuration or fall back to all public properties with a getter.
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
    Task<int> ResolveHashAsync<T>(T value);

    /// <summary>
    /// HashCode Resolver service.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <remarks>
    /// Same behavior as <see cref="ResolveHash{T}"/> but will check if any properties are of type <see cref="Task{T}"/> and await their values instead of getting a hashcode for the task instance.
    /// </remarks>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="propertyNames">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    Task<int> ResolveHashAsync<T>(T value, params string[] propertyNames);

    /// <summary>
    /// HashCode Resolver service.
    /// Default behavior will use all public properties with a getter, as long as their case-sensitive names are included in the <paramref name="propertyNames"/> list. This does not overrule the <see cref="HasherExcludeAttribute"/>.
    /// <see cref="HasherIncludeAttribute"/> can be used to include non-public ones.
    /// <see cref="HasherExcludeAttribute"/> can be used to exclude public ones.
    /// </summary>
    /// <remarks>
    /// Same behavior as <see cref="ResolveHash{T}"/> but will check if any properties are of type <see cref="Task{T}"/> and await their values instead of getting a hashcode for the task instance.
    /// </remarks>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="value">The entity to get a hashcode for</param>
    /// <param name="properties">The properties to use, rather than all of them.</param>
    /// <returns>A hash code for this value based on its properties</returns>
    Task<int> ResolveHashAsync<T>(T value, params Func<T, Task<object?>>[] properties);
}
