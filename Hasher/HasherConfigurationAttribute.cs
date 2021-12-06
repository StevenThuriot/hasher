namespace SimpleHasher;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class HasherConfigurationAttribute : Attribute
{
    public HasherConfigurationAttribute(params string[] properties)
    {
        if (properties is null || properties.Length == 0)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        Properties = properties.ToHashSet();
    }

    public IEnumerable<string> Properties { get; }
}