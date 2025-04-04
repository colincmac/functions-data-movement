using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.DataMovement;
using Azure.Storage.DataMovement.Blobs;
using Azure.Storage.DataMovement.Files.Shares;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class BlobTrigger1
    {
        private readonly ILogger<BlobTrigger1> _logger;
        private readonly ShareClient _shareClient;

        public BlobTrigger1(ShareClient shareClient, ILogger<BlobTrigger1> logger)
        {
            _logger = logger;
            _shareClient = shareClient;
        }

        [Function(nameof(BlobTrigger1))]
        public async Task Run(
            [BlobTrigger("samples-workitems/{blobName}", Connection = "SourceBlobStorageConnectionString")] BlockBlobClient blobClient, 
            string blobName)
        {
            // https://github.com/Azure/azure-sdk-for-net/blob/75ef89b35d4905cd9a6c324eb6e8d39fdaa95b7c/sdk/storage/Azure.Storage.DataMovement/samples/Sample01b_Migration.cs#L315;
            // We are targeting a file share file with the same name as the blob name
            var shareFileClient = _shareClient
                .GetDirectoryClient("samples-workitems")
                .GetFileClient(blobName);

            var fileShare = ShareFilesStorageResourceProvider.FromClient(shareFileClient);
            var blob = BlobsStorageResourceProvider.FromClient(blobClient);

            var transferManager = new TransferManager();
            var operation = await transferManager.StartTransferAsync(blob, fileShare);
            await operation.WaitForCompletionAsync();
            _logger.LogInformation($"C# Blob trigger function transfered blob to file share file \n Blob: {blob.Uri} \n Share: {shareFileClient.Uri}");
        }
    }
}
