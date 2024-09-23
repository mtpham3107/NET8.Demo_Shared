using Microsoft.AspNetCore.Http;
using NET8.Demo.Shared;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.RemoteServices.Services.Implements;

public class RemoteService(HttpClient httpClient) : IRemoteService
{
    public async ValueTask<TResponse> GetAsync<TResponse>(string endpoint, string token = null, IDictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return DeserializeObject<TResponse>(responseContent);
    }

    public async ValueTask<TResponse> PostAsync<TResponse>(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return DeserializeObject<TResponse>(responseContent);
    }

    public async ValueTask<TResponse> PostAsync<TResponse>(string endpoint, IFormFile file, string token = null, IDictionary<string, string> headers = null)
    {
        using var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        form.Add(fileContent, "file", file.FileName);

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = form;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return DeserializeObject<TResponse>(responseContent);
    }

    public async ValueTask<TResponse> PostAsync<TResponse>(string endpoint, ICollection<IFormFile> files, string token = null, IDictionary<string, string> headers = null)
    {
        using var form = new MultipartFormDataContent();

        if (files != null)
        {
            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                form.Add(fileContent, "files", file.FileName);
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = form;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return DeserializeObject<TResponse>(responseContent);
    }

    public async ValueTask<TResponse> PutAsync<TResponse>(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return DeserializeObject<TResponse>(responseContent);
    }

    public async ValueTask DeleteAsync(string endpoint, string token = null, IDictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
    }

    public async ValueTask DeleteAsync(string endpoint, object payload, string token = null, IDictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = DeserializeObject<ErrorResponse>(errorContent)?.Error;
            throw new BusinessException(errorResponse?.Code, errorResponse?.Message, errorResponse?.Detail);
        }

        response.EnsureSuccessStatusCode();
    }
}
