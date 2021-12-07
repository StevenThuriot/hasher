using Microsoft.Extensions.DependencyInjection;
using SimpleHasher.Internals;

namespace SimpleHasher;

public static class ServiceRegistratrions
{
    public static IServiceCollection AddHasher(this IServiceCollection serviceCollection) => serviceCollection.AddHasher(_ => { });

    public static IServiceCollection AddHasher(this IServiceCollection serviceCollection, Action<IHashOptions> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        HashOptions options = new();
        configure(options);

        return serviceCollection.AddSingleton<HashOptions>(options)
                                .AddSingleton<IHashService, HashService>();
    }
}