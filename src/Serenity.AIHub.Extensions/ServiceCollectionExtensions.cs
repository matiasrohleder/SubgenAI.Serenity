using Microsoft.Extensions.DependencyInjection;
using Serenity.AIHub.Client;
using Serenity.AIHub.Models;

namespace Serenity.AIHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerenityAIHub(
        this IServiceCollection services,
        Action<SerenityAIHubOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<SerenityAIHubOptions>();
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}