namespace SimpleHasher.Internals;

internal static class TaskValueResolver
{
    private static readonly Dictionary<Type, Func<Task, object?>> s_resolvers = new();
    private static readonly object s_lock = new();

    public static async Task<object?> Get(Task task)
    {
        var taskType = task.GetType();
        if (!taskType.IsGenericType)
        {
            return null;
        }

        if (!task.IsCompleted)
        {
            await task.ConfigureAwait(false);
        }

        if (!s_resolvers.TryGetValue(taskType, out Func<Task, object?>? resolver))
        {
            lock (s_lock)
            {
                if (!s_resolvers.TryGetValue(taskType, out resolver))
                {
                    var getMethod = taskType.GetProperty("Result")?.GetMethod;
                    if (getMethod is null)
                    {
                        s_resolvers[taskType] = resolver = _ => null;
                    }
                    else
                    {
                        ParameterExpression? parameter = Expression.Parameter(typeof(Task));
                        var lambda = Expression.Lambda<Func<Task, object>>(
                            Expression.Convert(
                                Expression.Call(
                                    Expression.Convert(parameter, taskType),
                                    getMethod
                                ),
                                typeof(object)
                            ),
                            parameter
                        );

                        s_resolvers[taskType] = resolver = lambda.Compile();
                    }
                }
            }
        }

        return resolver(task);
    }
}