using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Utilities.Configurations;

public static class ConfigurationExtensions
{
    public static IServiceCollection BindConfiguration<TConfig>(this IServiceCollection services, string section)
        where TConfig : class, new()
    {
        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var instance = new TConfig();
            config
                .GetSection(section)
                .Bind(instance);
            return instance;
        });
        return services;
    }
}