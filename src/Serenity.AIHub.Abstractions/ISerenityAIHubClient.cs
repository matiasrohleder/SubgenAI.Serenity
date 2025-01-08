using System.Threading;
using System.Threading.Tasks;

namespace Serenity.AIHub.Abstractions;

public interface ISerenityAIHubClient
{
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
}