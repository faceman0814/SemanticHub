namespace FaceMan.SemanticHub
{
    public class AppConfiguration
    {
        public AzureOpenAIConfig AzureOpenAI { get; set; }
        public AzureOpenAIConfig AzureOpenAIImages { get; set; }
        public AzureOpenAIEmbeddingsConfig AzureOpenAIEmbeddings { get; set; }
        public AzureAISearchConfig AzureAISearch { get; set; }
        public QdrantConfig Qdrant { get; set; }
        public WeaviateConfig Weaviate { get; set; }
        public KeyVaultConfig KeyVault { get; set; }
        public HuggingFaceConfig HuggingFace { get; set; }
        public PineconeConfig Pinecone { get; set; }
        public BingConfig Bing { get; set; }
        public GoogleConfig Google { get; set; }
        public GithubConfig Github { get; set; }
        public PostgresConfig Postgres { get; set; }
        public RedisConfig Redis { get; set; }
        public JiraConfig Jira { get; set; }
        public ChromaConfig Chroma { get; set; }
        public KustoConfig Kusto { get; set; }
        public MongoDBConfig MongoDB { get; set; }

    }

    public class ModelBasicConfig
    {
        public string ModelId { get; set; }
        public string DeploymentName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }
    public class OpenAIConfig
    {
        public string ModelId { get; set; }
        public string ChatModelId { get; set; }
        public string EmbeddingModelId { get; set; }
        public string ApiKey { get; set; }
    }

    public class AzureOpenAIConfig
    {
        public string ServiceId { get; set; }
        public string DeploymentName { get; set; }
        public string ModelId { get; set; }
        public string ChatDeploymentName { get; set; }
        public string ChatModelId { get; set; }
        public string ImageDeploymentName { get; set; }
        public string ImageModelId { get; set; }
        public string ImageEndpoint { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string ImageApiKey { get; set; }
        public string ApiVersion { get; set; }
    }

    public class AzureOpenAIEmbeddingsConfig
    {
        public string DeploymentName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }

    public class AzureAISearchConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string IndexName { get; set; }
    }

    public class QdrantConfig
    {
        public string Endpoint { get; set; }
        public string Port { get; set; }
    }

    public class WeaviateConfig
    {
        public string Scheme { get; set; }
        public string Endpoint { get; set; }
        public string Port { get; set; }
        public string ApiKey { get; set; }
    }

    public class KeyVaultConfig
    {
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class HuggingFaceConfig
    {
        public string ApiKey { get; set; }
        public string ModelId { get; set; }
    }

    public class PineconeConfig
    {
        public string ApiKey { get; set; }
        public string Environment { get; set; }
    }

    public class BingConfig
    {
        public string ApiKey { get; set; }
    }

    public class GoogleConfig
    {
        public string ApiKey { get; set; }
        public string SearchEngineId { get; set; }
    }

    public class GithubConfig
    {
        public string PAT { get; set; }
    }

    public class PostgresConfig
    {
        public string ConnectionString { get; set; }
    }

    public class RedisConfig
    {
        public string Configuration { get; set; }
    }

    public class JiraConfig
    {
        public string ApiKey { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
    }

    public class ChromaConfig
    {
        public string Endpoint { get; set; }
    }

    public class KustoConfig
    {
        public string ConnectionString { get; set; }
    }

    public class MongoDBConfig
    {
        public string ConnectionString { get; set; }
    }
}