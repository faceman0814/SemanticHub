﻿using FaceMan.SemanticHub.Generation.ImageGeneration;

using Microsoft.SemanticKernel.ChatCompletion;

namespace FaceMan.SemanticHub
{
    public class AppConfiguration
    {
        public SemanticHubAzureOpenAIConfig AzureOpenAI { get; set; }
        public SemanticHubTongYiConfig TongYi { get; set; }
        public SemanticHubZhiPuConfig ZhiPu { get; set; }
        public SemanticHubWenXinConfig WenXin { get; set; }
        public SemanticHubXunFeiConfig XunFei { get; set; }
        public SemanticHubAzureAISearchConfig AzureAISearch { get; set; }
        public SemanticHubQdrantConfig Qdrant { get; set; }
        public SemanticHubWeaviateConfig Weaviate { get; set; }
        public SemanticHubKeyVaultConfig KeyVault { get; set; }
        public SemanticHubHuggingFaceConfig HuggingFace { get; set; }
        public SemanticHubPineconeConfig Pinecone { get; set; }
        public SemanticHubBingConfig Bing { get; set; }
        public SemanticHubGoogleConfig Google { get; set; }
        public SemanticHubGithubConfig Github { get; set; }
        public SemanticHubPostgresConfig Postgres { get; set; }
        public SemanticHubRedisConfig Redis { get; set; }
        public SemanticHubJiraConfig Jira { get; set; }
        public SemanticHubChromaConfig Chroma { get; set; }
        public SemanticHubKustoConfig Kusto { get; set; }
        public SemanticHubMongoDBConfig MongoDB { get; set; }
    }
    public class SemanticHubZhiPuConfig
    {
        public string ModelName { get; set; }
        public string Secret { get; set; }
        public string Endpoint { get; set; }
    }
    public class SemanticHubXunFeiConfig
    {
        public string ModelName { get; set; } = "Spark V3.5";
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string AppId { get; set; }
    }

    public class SemanticHubWenXinConfig
    {
        public string ModelName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
    }

    public class SemanticHubTongYiConfig
    {
        public ImageParameters ImageParameters { get; set; }
        public string ModelName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }
    public class SemanticHubOpenAIConfig
    {
        public string ModelName { get; set; }
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }

    public class SemanticHubAzureOpenAIConfig
    {
        public string DeploymentName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string ApiVersion { get; set; }
    }

    public class SemanticHubAzureAISearchConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string IndexName { get; set; }
    }

    public class SemanticHubQdrantConfig
    {
        public string Endpoint { get; set; }
        public string Port { get; set; }
    }

    public class SemanticHubWeaviateConfig
    {
        public string Scheme { get; set; }
        public string Endpoint { get; set; }
        public string Port { get; set; }
        public string ApiKey { get; set; }
    }

    public class SemanticHubKeyVaultConfig
    {
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class SemanticHubHuggingFaceConfig
    {
        public string ApiKey { get; set; }
        public string ModelId { get; set; }
    }

    public class SemanticHubPineconeConfig
    {
        public string ApiKey { get; set; }
        public string Environment { get; set; }
    }

    public class SemanticHubBingConfig
    {
        public string ApiKey { get; set; }
    }

    public class SemanticHubGoogleConfig
    {
        public string ApiKey { get; set; }
        public string SearchEngineId { get; set; }
    }

    public class SemanticHubGithubConfig
    {
        public string PAT { get; set; }
    }

    public class SemanticHubPostgresConfig
    {
        public string ConnectionString { get; set; }
    }

    public class SemanticHubRedisConfig
    {
        public string Configuration { get; set; }
    }

    public class SemanticHubJiraConfig
    {
        public string ApiKey { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
    }

    public class SemanticHubChromaConfig
    {
        public string Endpoint { get; set; }
    }

    public class SemanticHubKustoConfig
    {
        public string ConnectionString { get; set; }
    }

    public class SemanticHubMongoDBConfig
    {
        public string ConnectionString { get; set; }
    }
}