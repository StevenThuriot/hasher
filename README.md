# Hasher

[![NuGet version (SimpleHasher)](https://img.shields.io/nuget/v/SimpleHasher?logo=nuget)](https://www.nuget.org/packages/SimpleHasher/)


Simple Hashing helper for entities. Default behaviour calculates the `HashCode` for all public properties.

`HasherIncludeAttribute` can be used to include non-public ones.

`HasherExcludeAttribute` can be used to exclude public ones.

## Usage

### Normal

```csharp
using Hasher;

public class Entity 
{
    public int Property1 { get; set; }
    public int Property2 { set => Console.Write(value); } // Skipped
    private int Property3 { get; set; } // Skipped
    [HasherInclude] private int Property4 { get; set; }
    [HasherExclude] public int Property5 { get; set; } // Skipped
}

var entity = new Entity();
var hashCode = entity.ResolveHash();
```

### Async

```csharp
using Hasher;

public class EntityWithTasks
{
    public Task<int> Property1 { get; set; } // Will await this value
    public int Property2 { set => Console.Write(value); } // Skipped
    private int Property3 { get; set; } // Skipped
    [HasherInclude] private int Property4 { get; set; }
    [HasherExclude] public int Property5 { get; set; } // Skipped
}

var entity = new EntityWithTasks();
var hashCode = await entity.ResolveHashAsync();
```

By using `ResolveHashAsync` we will get the `HashCode` of the actual `Task` value, if any, instead of the `Task`'s `HashCode`.

### As a service

```csharp
serviceCollection.AddHasher();

...

public class MyService
{
    private readonly IHasher _hasher;_
    public MyService(IHasher hasher)
    {
        _hasher = hasher;_
    }

    public  void DoWork<T>(T value)
    {
        var hash = _hasher.ResolveHash(value);
    }
}
```

### Null

Returns `0` for null values.

```
using Hasher;

string nullString = null;
var hashCode = nullString.ResolveHash(); // == 0
```

### Nested vs Top Level Only

You can enable nested calculations during registration using:

```csharp
serviceCollection.AddHasher(o => o.EnableNestedHashes());
```


### Custom Mappings

Custom mappings are allowed for certain types. You can configure your mappings using:

```csharp
serviceCollection.AddHasher(o => o.Register<string>(x => x.Length, x => x.Count(s => s == 's'), x => x.Count(s => s == 'd'))
                                  .Register<Entity>(x => x.Property1, x => x.Property2));
```

This, obviously, only works when using the injected service (`IHashService`). Simpler mapping has been supplied for the extension methods by using the `HasherConfiguration` attribute which offers support for top level property names:

```csharp
[HasherConfiguration(nameof(MyProperty1), nameof(MyProperty3))]
public class MyClass3
{
    public int MyProperty1 { get; set; }
    public int MyProperty2 { get; set; } // Skipped
    public int MyProperty3 { get; set; }
}
```