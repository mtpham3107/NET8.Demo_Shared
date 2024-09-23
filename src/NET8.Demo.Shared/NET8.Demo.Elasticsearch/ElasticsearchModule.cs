using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace NET8.Demo.Elasticsearch;

public static class ElasticsearchModule
{
    public static void ConfigureSerilog(string applicationName)
    {
        var elasticsearchUrl = "http://localhost:9200";
        var indexFormat = "logs-{0:yyyy.MM.dd}";

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
                ModifyConnectionSettings = x => x.BasicAuthentication("username", "password")
            })
            .CreateLogger();
    }
}
