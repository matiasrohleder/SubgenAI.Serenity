using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.AIHub.Client;
using Serenity.AIHub.Models;

namespace Serenity.AIHub.Extensions;

/// <summary>
/// Provides extension methods for the IServiceCollection interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Serenity AI Hub services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configureOptions">An action that configures the Serenity AI Hub options.</param>
    /// <returns>The IServiceCollection with the Serenity AI Hub services added.</returns>
    public static IServiceCollection AddSerenityAIHub(
        this IServiceCollection services,
        Action<SerenityAIHubOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SerenityAIHubOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}