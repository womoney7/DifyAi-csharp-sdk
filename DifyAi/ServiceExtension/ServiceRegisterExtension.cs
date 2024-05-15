using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace DifyAi.ServiceExtension;

public static class ServiceRegisterExtension
{
    /// <summary>
    ///     Register DifyAi services
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IServiceCollection AddDifyAiServices(this IServiceCollection serviceCollection)
    {
        var services = serviceCollection;


        using var scope = services.BuildServiceProvider().CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        services.Configure<DifyAiConfigDto>(configuration.GetSection("DifyAi"));


        var botApiKey = configuration.GetSection("DifyAi:BotApiKey").Value;
        var baseUrl = configuration.GetSection("DifyAi:BaseUrl").Value;
        var proxyConfig = configuration.GetSection("DifyAi:ProxyConfig").Value;

        if (string.IsNullOrEmpty(botApiKey))
        {
            throw new DifyConfigMissingException("Missing base url!");
        }

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new DifyConfigMissingException("Missing api key!");
        }

        if (botApiKey.StartsWith("Bearer ")) botApiKey = botApiKey.Replace("Bearer ", "");

        services.AddHttpClient("DifyAi.Bot",
            client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "IcedMango/DifyAi-Sdk(DifyAi.Bot)");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {botApiKey}");
            }).ConfigurePrimaryHttpMessageHandler(() => string.IsNullOrWhiteSpace(proxyConfig)
            ? new HttpClientHandler()
            : new HttpClientHandler
            {
                Proxy = new WebProxy(proxyConfig)
                {
                    UseDefaultCredentials = false
                },
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        //register service

        services.AddTransient<IDifyAiChatServices, DifyAiChatServices>();
        services.AddTransient<IRequestExtension, RequestExtension>();


        return services;
    }
}