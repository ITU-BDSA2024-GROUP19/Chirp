using Azure.Storage.Blobs;

namespace Chirp.Infrastructure.External;

public interface IBlobServiceClient
{
    BlobContainerClient GetBlobContainerClient(string BlobContainerName);
}

public class AzureBlobServiceClient : IBlobServiceClient
{
    private readonly BlobServiceClient? _client;

    public AzureBlobServiceClient(BlobServiceClient? blobServiceClient = null)
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
}