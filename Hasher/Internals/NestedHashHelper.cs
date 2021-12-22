namespace SimpleHasher.Internals;

internal static class NestedHashHelper
{
    public static int ResolveNestedHash(object propertyValue, HashService? service)
    {
        if (service is null)
        {
#if NETSTANDARD2_1
            return HashExtensionResolver.Get(propertyValue);
#else
            dynamic dynamicProperty = propertyValue;
            return (int)Extensions.HashExtensions.ResolveHash(dynamicProperty);
#endif
        }
        else
        {
#if NETSTANDARD2_1
            return HashExtensionResolver.Get(propertyValue, service);
#else
            dynamic dynamicProperty = propertyValue;
            return (int)service.ResolveHash(dynamicProperty);
#endif
        }
    }

    public static Task<int> ResolveNestedHashAsync(object propertyValue, HashService? service)
    {
        if (service is null)
        {
#if NETSTANDARD2_1
            return HashExtensionResolver.GetAsync(propertyValue);
#else
            dynamic dynamicProperty = propertyValue;
            return (Task<int>)Extensions.HashExtensions.ResolveHashAsync(dynamicProperty);
#endif
        }
        else
        {
#if NETSTANDARD2_1
            return HashExtensionResolver.GetAsync(propertyValue, service);
#else
            dynamic dynamicProperty = propertyValue;
            return (Task<int>)service.ResolveHashAsync(dynamicProperty);
#endif
        }
    }
}
