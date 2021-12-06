namespace SimpleHasher.Internals;

internal static class NestedHashHelper
{
    public static object ResolveNestedHash(object propertyValue, HashService service)
    {
#if IsDotNetStandard
        return HashExtensionResolver.Get(propertyValue, service);
#else
        dynamic dynamicProperty = propertyValue;
        return (int)service.ResolveHash(dynamicProperty);
#endif
    }

    public static Task<int> ResolveNestedHashAsync(object propertyValue, HashService service)
    {
#if IsDotNetStandard
        return HashExtensionResolver.GetAsync(propertyValue, service);
#else
        dynamic dynamicProperty = propertyValue;
        return (Task<int>)service.ResolveHashAsync(dynamicProperty);
#endif
    }
}
