namespace NET8.Demo.RemoteServices.Services;

public interface IHttpClientService
{
    IRemoteService GetRemoteService(string clientName);
}
