using Microsoft.AspNetCore.Http;

namespace NET8.Demo.RemoteServices.Services;

public interface IRemoteService
{
    ValueTask<T> GetAsync<T>(string endpoint, string token = null, IDictionary<string, string> headers = null);

    ValueTask<T> PostAsync<T>(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null);

    ValueTask<TResponse> PostAsync<TResponse>(string endpoint, IFormFile file, string token = null, IDictionary<string, string> headers = null);

    ValueTask<TResponse> PostAsync<TResponse>(string endpoint, ICollection<IFormFile> files, string token = null, IDictionary<string, string> headers = null);

    ValueTask<T> PutAsync<T>(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null);

    ValueTask DeleteAsync(string endpoint, string token = null, IDictionary<string, string> headers = null);

    ValueTask DeleteAsync(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null);
}
