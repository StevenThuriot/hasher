#if NETSTANDARD2_1

using SimpleHasher.Extensions;
using SimpleHasher.Internals;

namespace SimpleHasher;

internal static class HashExtensionResolver
{
    private static readonly Dictionary<Type, Func<HashService, object, int>> s_hasher = new();
    private static readonly Dictionary<Type, Func<HashService, object, Task<int>>> s_asyncHasher = new();
    private static readonly Dictionary<Type, Func<object, int>> s_generic_hasher = new();
    private static readonly Dictionary<Type, Func<object, Task<int>>> s_generic_asyncHasher = new();


    internal static int Get(object propertyValue)
    {
        var propertyType = propertyValue.GetType();

        if (!s_generic_hasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_generic_hasher)
            {
                if (!s_generic_hasher.TryGetValue(propertyType, out @delegate))
                {
                    var methodInfo = typeof(HashExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.Name == nameof(HashExtensions.ResolveHash) && x.GetParameters().Length == 1);
                    var genericMethod = methodInfo.MakeGenericMethod(propertyType);

                    ParameterExpression valueParameter = Expression.Parameter(typeof(object));
                    var lambda = Expression.Lambda<Func<object, int>>(
                        Expression.Call(
                            genericMethod,
                            Expression.Convert(valueParameter, propertyType)
                        ),
                        valueParameter
                    );

                    s_generic_hasher[propertyType] = @delegate = lambda.Compile();
                }
            }
        }

        return @delegate(propertyValue);
    }

    internal static int Get(object propertyValue, HashService service)
    {
        var propertyType = propertyValue.GetType();

        if (!s_hasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_hasher)
            {
                if (!s_hasher.TryGetValue(propertyType, out @delegate))
                {
                    var methodInfo = typeof(HashService).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => x.Name == nameof(HashService.ResolveHash) && x.GetParameters().Length == 1);
                    var genericMethod = methodInfo.MakeGenericMethod(propertyType);

                    ParameterExpression valueParameter = Expression.Parameter(typeof(object));
                    ParameterExpression serviceParameter = Expression.Parameter(typeof(HashService));
                    var lambda = Expression.Lambda<Func<HashService, object, int>>(
                        Expression.Call(
                            serviceParameter,
                            genericMethod,
                            Expression.Convert(valueParameter, propertyType)
                        ),
                        serviceParameter,
                        valueParameter
                    );

                    s_hasher[propertyType] = @delegate = lambda.Compile();
                }
            }
        }

        return @delegate(service, propertyValue);
    }

    internal static Task<int> GetAsync(object propertyValue)
    {
        var propertyType = propertyValue.GetType();

        if (!s_generic_asyncHasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_generic_asyncHasher)
            {
                if (!s_generic_asyncHasher.TryGetValue(propertyType, out @delegate))
                {
                    var methodInfo = typeof(HashExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.Name == nameof(HashExtensions.ResolveHashAsync) && x.GetParameters().Length == 1);
                    var genericMethod = methodInfo.MakeGenericMethod(propertyType);

                    ParameterExpression valueParameter = Expression.Parameter(typeof(object));
                    var lambda = Expression.Lambda<Func<object, Task<int>>>(
                        Expression.Call(
                            genericMethod,
                            Expression.Convert(valueParameter, propertyType)
                        ),
                        valueParameter
                    );

                    s_generic_asyncHasher[propertyType] = @delegate = lambda.Compile();
                }
            }
        }

        return @delegate(propertyValue);
    }

    internal static Task<int> GetAsync(object propertyValue, HashService service)
    {
        var propertyType = propertyValue.GetType();

        if (!s_asyncHasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_asyncHasher)
            {
                if (!s_asyncHasher.TryGetValue(propertyType, out @delegate))
                {
                    var methodInfo = typeof(HashService).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => x.Name == nameof(HashService.ResolveHashAsync) && x.GetParameters().Length == 1);
                    var genericMethod = methodInfo.MakeGenericMethod(propertyType);

                    ParameterExpression valueParameter = Expression.Parameter(typeof(object));
                    ParameterExpression serviceParameter = Expression.Parameter(typeof(HashService));
                    var lambda = Expression.Lambda<Func<HashService, object, Task<int>>>(
                        Expression.Call(
                            serviceParameter,
                            genericMethod,
                            Expression.Convert(valueParameter, propertyType)
                        ),
                        serviceParameter,
                        valueParameter
                    );

                    s_asyncHasher[propertyType] = @delegate = lambda.Compile();
                }
            }
        }

        return @delegate(service, propertyValue);
    }
}
#endif