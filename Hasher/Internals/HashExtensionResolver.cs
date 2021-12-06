#if IsDotNetStandard

using SimpleHasher.Extensions;
using SimpleHasher.Internals;

namespace SimpleHasher;

internal static class HashExtensionResolver
{
    private static readonly Dictionary<Type, Func<HashService, object, int>> s_hasher = new();
    private static readonly Dictionary<Type, Func<HashService, object, Task<int>>> s_asyncHasher = new();
    private static readonly object s_lock = new();

    internal static int Get(object propertyValue, HashService service)
    {
        var propertyType = propertyValue.GetType();

        if (!s_hasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_lock)
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

    internal static Task<int> GetAsync(object propertyValue, HashService service)
    {
        var propertyType = propertyValue.GetType();

        if (!s_asyncHasher.TryGetValue(propertyType, out var @delegate))
        {
            lock (s_lock)
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