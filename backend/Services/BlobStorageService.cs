using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using backend.Contracts.Responses;
using backend.Options;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class BlobStorageService
{
    private readonly BlobStorage _blobStorageOptions;

    public BlobStorageService(IOptions<BlobStorage> blobStorageOptions)
    {
        _blobStorageOptions = blobStorageOptions.Value;
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var containerClient = new BlobContainerClient(_blobStorageOptions.ConnectionString, _blobStorageOptions.ContainerName);

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(file.FileName);

        await using var sream = file.OpenReadStream();
        
        await blobClient.UploadAsync(
            sream,
            new BlobHttpHeaders
            {
                ContentType = file.ContentType
            },
            cancellationToken: cancellationToken);
        
        return blobClient.Uri.ToString();
    }

    public async Task<FileResponse> DownAsync(string name, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = new BlobContainerClient(_blobStorageOptions.ConnectionString, _blobStorageOptions.ContainerName);

        BlobClient blobClient = containerClient.GetBlobClient(name);

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);

        var bytes = response.Value.Content.ToArray();
        var contentType = response.Value.Details.ContentType;

        return new FileResponse(bytes, contentType);
    }
}