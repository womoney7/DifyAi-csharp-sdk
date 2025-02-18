using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http;
using System.ClientModel.Primitives;
using Microsoft.Extensions.Options;
using IcedMango.DifyAi.Services;

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

        var botApiKey = configuration.GetSection("DifyAi:BotApiKey").Value;
        var datasetApiKey = configuration.GetSection("DifyAi:DatasetApiKey").Value;
        var baseUrl = configuration.GetSection("DifyAi:BaseUrl").Value;
        var proxyConfig = configuration.GetSection("DifyAi:ProxyConfig").Value;
        var subscriptionKey = configuration.GetSection("DifyAi:Ocp-Apim-Subscription-Key").Value;
        if (string.IsNullOrEmpty(botApiKey))
        {
            throw new DifyConfigMissingException("Missing api key!");
        }

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new DifyConfigMissingException("Missing base url!");
        }

        if (botApiKey.StartsWith("Bearer ")) botApiKey = botApiKey.Replace("Bearer ", "");

        services.AddHttpClient("DifyAi.Bot",
            client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "IcedMango/DifyAiSdk");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {botApiKey}");
                if (!string.IsNullOrEmpty(subscriptionKey))
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                }
            }).ConfigurePrimaryHttpMessageHandler(() => string.IsNullOrWhiteSpace(proxyConfig)
            ? new HttpClientHandler()
            : new HttpClientHandler
            {
                Proxy = new WebProxy(proxyConfig)
                {
                    UseDefaultCredentials = false
                },
#if !NETSTANDARD2_0
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
            });

        //add dataset api client
        if (!string.IsNullOrWhiteSpace(datasetApiKey))
        {
            if (datasetApiKey.StartsWith("Bearer ")) datasetApiKey = datasetApiKey.Replace("Bearer ", "");

            services.AddHttpClient("DifyAi.Dataset",
                client =>
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "IcedMango/DifyAiSdk");
                    client.BaseAddress = new Uri(baseUrl);

                    client.DefaultRequestHeaders.Authorization =
                        AuthenticationHeaderValue.Parse($"Bearer {datasetApiKey}");

                    if (!string.IsNullOrEmpty(subscriptionKey))
                    {
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                    }

                }).ConfigurePrimaryHttpMessageHandler(() => string.IsNullOrWhiteSpace(proxyConfig)
                ? new HttpClientHandler()
                : new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyConfig)
                    {
                        UseDefaultCredentials = false
                    },
#if !NETSTANDARD2_0
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
                });
        }


        //register service
        services.AddTransient<IRequestExtension, RequestExtension>();
        services.AddSingleton<ClientPipeline>(isp =>
        {
            List<PipelinePolicy> list = new List<PipelinePolicy>();
            list.Add(ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(new System.ClientModel.ApiKeyCredential(botApiKey), "Authorization", "Bearer"));
            if (!string.IsNullOrEmpty(subscriptionKey))
            {
                list.Add(ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(new System.ClientModel.ApiKeyCredential(subscriptionKey), "Ocp-Apim-Subscription-Key"));
            }
            var clientPipeline = ClientPipeline
            .Create(new ClientPipelineOptions() { RetryPolicy = new ClientRetryPolicy(0), NetworkTimeout = new TimeSpan(0, 0, 30) },
                perCallPolicies: [],
                perTryPolicies: new ReadOnlySpan<PipelinePolicy>(list.ToArray()),
                beforeTransportPolicies: []
            );

            return clientPipeline;
        });
        services.AddTransient<IDifyAiChatServices, DifyAiChatServices>();
        services.AddTransient<IDifyAiDatasetServices, DifyAiDatasetServices>();
        services.AddTransient<IDifyAiWorkflowServices, DifyAiWorkflowServices>();

        return services;
    }
}