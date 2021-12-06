namespace SimpleHasher.Internals;

internal static class DefaultHasher<T>
{
    private static readonly IDictionary<string, (Lazy<Func<T, object?>> getter, bool isTask)> s_getters = Build();

    private static Dictionary<string, (Lazy<Func<T, object?>> compiled, bool isTask)> Build()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(x => x.CanRead && x.GetMethod != null);

        var classAttribute = typeof(T).GetCustomAttribute<HasherConfigurationAttribute>();
        if (classAttribute is null)
        {
            properties = properties.Select(x => new
            {
                attributes = x.GetCustomAttributes().OfType<HasherAttribute>().ToHashSet(),
                property = x
            })
            .Where(x => !x.attributes.OfType<HasherExcludeAttribute>().Any())
            .Where(x => x.property.GetMethod!.IsPublic || x.attributes.OfType<HasherIncludeAttribute>().Any())
            .Select(x => x.property);
        }
        else
        {
            properties = properties.Where(x => classAttribute.Properties.Contains(x.Name));
        }

        var result = properties.ToDictionary(x => x.Name, x =>
        {
            var lambda = BuildLambda(x.GetMethod!);
            var compiled = new Lazy<Func<T, object?>>(() => lambda.Compile());
            bool isTask = typeof(Task).IsAssignableFrom(x.PropertyType);
            return (compiled, isTask);
        });

        if (classAttribute is not null && classAttribute.Properties.Count() != result.Count)
        {
            throw new ArgumentException("Not all properties were found that are defined in the HasherConfigurationAttribute");
        }

        return result;
    }

    private static Expression<Func<T, object?>> BuildLambda(MethodInfo getMethod)
    {
        ParameterExpression? parameter = Expression.Parameter(typeof(T));

        return Expression.Lambda<Func<T, object?>>(
            Expression.Convert(
                Expression.Call(
                    parameter,
                    getMethod
                ),
                typeof(object)
            ),
            parameter
        );
    }

    public static int Get(T? value, HashService service, HashOptions options) => value is null ? 0 : GetInternal(value, service, options, s_getters.Values.Select(x => x.getter));

    public static int Get(T? value, HashService service, HashOptions options, params string[] propertyNames) => value is null ? 0 : GetInternal(value, service, options, ResolveGetters(propertyNames).Select(x => x.getter));

    public static async Task<int> GetAsync(T? value, HashService service, HashOptions options) => value is null ? 0 : await GetInternalAsync(value, service, options, s_getters.Values).ConfigureAwait(false);

    public static async Task<int> GetAsync(T? value, HashService service, HashOptions options, params string[] propertyNames) => value is null ? 0 : await GetInternalAsync(value, service, options, ResolveGetters(propertyNames)).ConfigureAwait(false);

    private static IEnumerable<(Lazy<Func<T, object?>> getter, bool isTask)> ResolveGetters(string[] propertyNames)
    {
        if (propertyNames is null || propertyNames.Length == 0)
        {
            return s_getters.Values;
        }

        return propertyNames.Select(x => s_getters[x]);
    }

    private static int GetInternal(T value, HashService service, HashOptions options, IEnumerable<Lazy<Func<T, object?>>> getters)
    {
        if (value is null)
        {
            return 0;
        }

        return SynchronousHasher<T>.GetInternal(value, service, options, getters.Select(x => x.Value));
    }

    private static Task<int> GetInternalAsync(T value, HashService service, HashOptions options, IEnumerable<(Lazy<Func<T, object?>> getter, bool isTask)> getters)
    {
        if (value is null)
        {
            return Task.FromResult(0);
        }

        return AsynchronousHasher<T>.GetInternal(value, service, options, getters.Select(x =>
        {
            if (!x.isTask)
            {
                return new Func<T, Task<object?>>(v => Task.FromResult(x.getter.Value(v)));
            }

            return new Func<T, Task<object?>>(async innerValue =>
            {
                object? propertyValue = x.getter.Value(innerValue);

                if (propertyValue is Task task)
                {
                    propertyValue = await TaskValueResolver.Get(task).ConfigureAwait(false);
                }

                return propertyValue;
            });
        }));
    }
}