using SimpleHasher.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace SimpleHasher.Tests;

public class Tests
{
    [Fact]
    public void Test()
    {
        var sut = new MyClass();
        int hash = sut.ResolveHash();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public async Task AsyncTest()
    {
        var sut = new MyClass();
        int hash = await sut.ResolveHashAsync();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void TestProperties()
    {
        var sut = new MyClass();
        int hash = sut.ResolveHash(x => x.MyProperty1, x => x.MyProperty3.MyProperty1);
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public async Task AsyncTestProperties()
    {
        var sut = new MyClass();
        int hash = await sut.ResolveHashAsync(nameof(MyClass.MyProperty2));
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void ConfigAttributeTest()
    {
        var sut = new MyClass3();
        int hash = sut.ResolveHash();
        Assert.NotEqual(0, hash);
    }
}

public class MyClass
{
    public int MyProperty1 { get; set; } = 1;
    public Task<int> MyProperty2 { get; set; } = Task.FromResult(2);
    public MyClass2 MyProperty3 { get; set; } = new();
}

public class MyClass2
{
    public int MyProperty1 { get; set; } = 3;
    public Task<int> MyProperty2 { get; set; } = Task.FromResult(4);
}

[HasherConfiguration(nameof(MyProperty1), nameof(MyProperty3))]
public class MyClass3
{
    public int MyProperty1 { get; set; }
    public int MyProperty2 { get; set; }
    public int MyProperty3 { get; set; }
}
