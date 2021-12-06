using SimpleHasher.Internals;
using System.Threading.Tasks;
using Xunit;

namespace SimpleHasher.Tests;

public class ServiceTests
{
    [Fact]
    public void Test()
    {
        var sut = new MyClass();

        var options = new HashOptions();
        options.EnableNestedHashes();
        options.Register<MyClass2>(x => x.MyProperty1);

        var service = new HashService(options);

        int hash = service.ResolveHash(sut);

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public async Task AsyncTest()
    {
        var sut = new MyClass();

        var options = new HashOptions();
        options.EnableNestedHashes();
        options.RegisterAsync<MyClass2>(x => Task.FromResult<object?>(x.MyProperty1), async x => await x.MyProperty2);

        var service = new HashService(options);

        int hash = await service.ResolveHashAsync(sut);

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void TestProperties()
    {
        var sut = new MyClass();

        var options = new HashOptions();
        var service = new HashService(options);

        int hash = service.ResolveHash(sut, x => x.MyProperty1, x => x.MyProperty3.MyProperty1);

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public async Task AsyncTestProperties()
    {
        var sut = new MyClass();

        var options = new HashOptions();
        var service = new HashService(options);

        int hash = await service.ResolveHashAsync(sut, nameof(MyClass.MyProperty2));

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void ConfigAttributeTest()
    {
        var sut = new MyClass3();

        var options = new HashOptions();
        var service = new HashService(options);

        int hash = service.ResolveHash(sut);

        Assert.NotEqual(0, hash);
    }
}
