using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.RemoteServices.Services;
using NET8.Demo.RemoteServices.Services.Implements;

namespace NET8.Demo.RemoteServices;

public static class RemoteServiceModule
{
    public static void ConfigureRemoteService(this IServiceCollection services)
    {
        var httpClientSettings = new Dictionary<string, string>()
        {
            {"GlobalAdmin", "https://localhost:7155" },
            {"Prodventory", "https://localhost:7156" },
            {"Ecom", "https://localhost:7157" },
        };

        foreach (var clientConfig in httpClientSettings)
        {
            var clientName = clientConfig.Key;
            var clientConfigValue = clientConfig.Value;

            services.AddHttpClient(clientConfig.Key, client =>
            {
                client.BaseAddress = new Uri(clientConfig.Value);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            });
        }

        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IRemoteService, RemoteService>();
    }
}
