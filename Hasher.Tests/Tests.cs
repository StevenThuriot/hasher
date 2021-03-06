using SimpleHasher.Extensions;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void HashList()
    {
        var sut = new MyClass();
        int hash = sut.MyProperty4.OrderBy(x => x.MyProperty1).Select(x => x.MyProperty2).ResolveHash();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void HashListComplicated()
    {
        var sut = new MyClass();
        int hash = sut.ResolveHash(x => x.MyProperty1, x => x.MyProperty4.OrderBy(x => x.MyProperty1).Select(x => x.MyProperty2));
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public async Task HashListAsync()
    {
        var sut = new MyClass();
        int hash = await sut.MyProperty4.OrderBy(x => x.MyProperty1).Select(x => x.MyProperty2).ResolveHashAsync();
        Assert.NotEqual(0, hash);
    }
}

public class MyClass
{
    public int MyProperty1 { get; set; } = 1;
    public Task<int> MyProperty2 { get; set; } = Task.FromResult(2);
    public MyClass2 MyProperty3 { get; set; } = new();
    public List<MyClass3> MyProperty4 { get; set; } = new List<MyClass3> { new MyClass3(), new MyClass3() };
    public string MyProperty5 { get; set; } = ";lkjgao;khgja;odkgjs;dlkgjsdlkgj;sldkjgjhs;dklgjh;sdklfgjh";
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
