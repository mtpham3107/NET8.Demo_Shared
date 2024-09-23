using NET8.Demo.RemoteServices.Services.Implements;

namespace NET8.Demo.RemoteServices.Services;

public class HttpClientService(IHttpClientFactory httpClientFactory) : IHttpClientService
{
    public IRemoteService GetRemoteService(string clientName)
    {
        var httpClient = httpClientFactory.CreateClient(clientName);
        return new RemoteService(httpClient);
    }
}
