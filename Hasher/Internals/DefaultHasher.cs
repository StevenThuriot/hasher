namespace SimpleHasher.Internals;

internal static class DefaultHasher<T>
{
    private static readonly IDictionary<string, HashInfo> s_info = Build();

    private class HashInfo
    {
        public HashInfo(PropertyInfo x)
        {
            var lambda = BuildLambda(x.GetMethod!);
            Getter = new Lazy<Func<T, object?>>(() => lambda.Compile());

            IsTask = typeof(Task).IsAssignableFrom(x.PropertyType);

            if (IsTask)
            {
                if (x.PropertyType.IsGenericType)
                {
                    //Assuming we won't inherit from task
                    var genericType = x.PropertyType.GetGenericArguments()[0];
                    IsEnumerable = genericType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(genericType);
                }
                else
                {
                    IsEnumerable = false;
                }
            }
            else
            {
                IsEnumerable = x.PropertyType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(x.PropertyType);
            }

            if (IsEnumerable)
            {
                ResolveByEnumeration = x.CustomAttributes.OfType<HashEnumeratedAttribute>().Any();
            }
        }

        public Lazy<Func<T, object?>> Getter { get; }
        public bool IsTask { get; }
        public bool IsEnumerable { get;}
        public bool ResolveByEnumeration { get;}
    }


    private static Dictionary<string, HashInfo> Build()
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

        var result = properties.ToDictionary(x => x.Name, x => new HashInfo(x));

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

    public static int Get(T? value, HashService service, IHashOptionsInternal options) => value is null ? 0 : GetInternal(value, service, options, s_info.Values);

    public static int Get(T? value, HashService service, IHashOptionsInternal options, params string[] propertyNames) => value is null ? 0 : GetInternal(value, service, options, ResolveInfo(propertyNames));

    public static async Task<int> GetAsync(T? value, HashService service, IHashOptionsInternal options) => value is null ? 0 : await GetInternalAsync(value, service, options, s_info.Values).ConfigureAwait(false);

    public static async Task<int> GetAsync(T? value, HashService service, IHashOptionsInternal options, params string[] propertyNames) => value is null ? 0 : await GetInternalAsync(value, service, options, ResolveInfo(propertyNames)).ConfigureAwait(false);

    private static IEnumerable<HashInfo> ResolveInfo(string[] propertyNames)
    {
        if (propertyNames is null || propertyNames.Length == 0)
        {
            return s_info.Values;
        }

        return propertyNames.Select(x => s_info[x]);
    }

    private static int GetInternal(T value, HashService service, IHashOptionsInternal options, IEnumerable<HashInfo> getters)
    {
        if (value is null)
        {
            return 0;
        }

        return SynchronousHasher<T>.GetInternal(value, service, options, getters.Select<DefaultHasher<T>.HashInfo, Func<T, object?>>(x =>
        {
            if ((options.IterateEnumerables || x.ResolveByEnumeration) && x.IsEnumerable)
            {
                return new Func<T, object?>(v => ResolveEnumerated(x, v, service));
            }
            else
            {
                return x.Getter.Value;
            }
        }));
    }

    private static Task<int> GetInternalAsync(T value, HashService service, IHashOptionsInternal options, IEnumerable<HashInfo> getters)
    {
        if (value is null)
        {
            return Task.FromResult(0);
        }

        return AsynchronousHasher<T>.GetInternal(value, service, options, getters.Select(x =>
        {
            if (!x.IsTask)
            {
                if ((options.IterateEnumerables || x.ResolveByEnumeration) && x.IsEnumerable)
                {
                    return new Func<T, Task<object?>>(v => ResolveEnumeratedAsync(x, v, service));
                }
                else
                {
                    return new Func<T, Task<object?>>(v => Task.FromResult(x.Getter.Value(v)));
                }
            }

            return new Func<T, Task<object?>>(async innerValue =>
            {
                object? propertyValue = x.Getter.Value(innerValue);

                if (propertyValue is Task task)
                {
                    propertyValue = await TaskValueResolver.Get(task).ConfigureAwait(false);
                }

                if ((options.IterateEnumerables || x.ResolveByEnumeration) && x.IsEnumerable)
                {
                    return ResolveEnumeratedAsync(propertyValue, service);
                }

                return propertyValue;
            });
        }));
    }

    private static object? ResolveEnumerated(HashInfo x, T v, HashService? service) => ResolveEnumerated(x.Getter.Value(v), service);

    private static object? ResolveEnumerated(object? resolvedValue, HashService? service)
    {
        if (resolvedValue is System.Collections.IEnumerable values)
        {
            SimpleHashCode hashCode = new();

            foreach (object? listValue in values)
            {
                hashCode.Add(NestedHashHelper.ResolveNestedHash(listValue, service));
            }

            return hashCode.ToHashCode();
        }

        return resolvedValue;
    }

    private static Task<object?> ResolveEnumeratedAsync(HashInfo x, T v, HashService? service) => ResolveEnumeratedAsync(x.Getter.Value(v), service);

    private static async Task<object?> ResolveEnumeratedAsync(object? resolvedValue, HashService? service)
    {
        if (resolvedValue is System.Collections.IEnumerable values)
        {
            SimpleHashCode hashCode = new();

            foreach (object? listValue in values)
            {
                hashCode.Add(await NestedHashHelper.ResolveNestedHashAsync(listValue, service));
            }

            return hashCode.ToHashCode();
        }

        return resolvedValue;
    }
}