using Azure.Storage.Blobs;

namespace Chirp.Infrastructure.External;

/// <summary>
/// Defines an optional connection to a Blob Container Client.
/// Made for use with Azure Storage.
/// </summary>
public interface IOptionalBlobServiceClient
{
    /// <summary>
    /// Makes call to BlobServiceClient.GetBlobContainerClient()
    /// </summary>
    /// <param name="blobContainerName"></param>
    /// <returns>A BlobContainerClient for the desired container.</returns>
    BlobContainerClient GetBlobContainerClient(string blobContainerName);

    /// <summary>
    /// Poll method for whether a BlobServiceClient is available.
    /// </summary>
    /// <returns><b>true</b> when a BlobServiceClient is available.</returns>
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