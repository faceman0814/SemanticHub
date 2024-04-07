using FaceMan.SemanticHub;
using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;
using FaceMan.SemanticHub.ModelExtensions.WenXin.Chat;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;

namespace Microsoft.SemanticKernel;
public static class SemanticHubServiceCollectionExtensions
{
    public static IKernelBuilder AddSemanticHubOpenAIChatCompletion(
       this IKernelBuilder builder,
       SemanticHubOpenAIConfig config,
       string? serviceId = "SemanticHubOpenAIChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ModelName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ApiKey);
        Func<IServiceProvider, object?, SemanticHubOpenAIChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }

    public static IKernelBuilder AddSemanticHubAzureOpenAIChatCompletion(
      this IKernelBuilder builder,
      SemanticHubAzureOpenAIConfig config,
      string? serviceId = "SemanticHubAzureOpenAIChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.DeploymentName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ApiKey);
        SemanticHubVerify.NotNullOrWhiteSpace(config.Endpoint);
        Func<IServiceProvider, object?, SemanticHubAzureOpenAIChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }

    public static IKernelBuilder AddSemanticHubWenXinChatCompletion(
     this IKernelBuilder builder,
     SemanticHubWenXinConfig config,
     string? serviceId = "SemanticHubWenXinChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ModelName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ApiKey);
        SemanticHubVerify.NotNullOrWhiteSpace(config.Secret);
        Func<IServiceProvider, object?, SemanticHubWenXinChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }

    public static IKernelBuilder AddSemanticHubXunFeiChatCompletion(
    this IKernelBuilder builder,
    SemanticHubXunFeiConfig config,
    string? serviceId = "SemanticHubXunFeiChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ModelName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ApiKey);
        SemanticHubVerify.NotNullOrWhiteSpace(config.AppId);
        SemanticHubVerify.NotNullOrWhiteSpace(config.Secret);
        Func<IServiceProvider, object?, SemanticHubXunFeiChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }

    public static IKernelBuilder AddSemanticHubZhiPuChatCompletion(
    this IKernelBuilder builder,
    SemanticHubZhiPuConfig config,
    string? serviceId = "SemanticHubZhiPuChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ModelName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.Secret);
        Func<IServiceProvider, object?, SemanticHubZhiPuChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }

    public static IKernelBuilder AddSemanticHubTongYiChatCompletion(
   this IKernelBuilder builder,
   SemanticHubTongYiConfig config,
   string? serviceId = "SemanticHubZhiPuChatCompletion")
    {
        SemanticHubVerify.NotNull(builder);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ModelName);
        SemanticHubVerify.NotNullOrWhiteSpace(config.ApiKey);
        Func<IServiceProvider, object?, SemanticHubTongYiChatCompletionService> factory = (serviceProvider, _) =>
            new(config, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);
        return builder;
    }
}
