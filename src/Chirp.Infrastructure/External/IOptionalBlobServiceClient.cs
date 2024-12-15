using Azure.Storage.Blobs;

namespace Chirp.Infrastructure.External;

public interface IOptionalBlobServiceClient
{
    BlobContainerClient GetBlobContainerClient(string BlobContainerName);
    bool IsAvailable();
}

public class OptionalBlobServiceClient : IOptionalBlobServiceClient
{
    private readonly BlobServiceClient? _client;

    public OptionalBlobServiceClient(BlobServiceClient? blobServiceClient = null)
    {
        _client = blobServiceClient;
    }

    public BlobContainerClient GetBlobContainerClient(string blobContainerName)
    {
        if (_client != null) 
        {
            return _client.GetBlobContainerClient(blobContainerName);
        }
        else
        {
            throw new InvalidOperationException("Azure Blob Service is unavailable");
        }
    }
    
    public bool IsAvailable()
    {
        return _client != null;
    }
}