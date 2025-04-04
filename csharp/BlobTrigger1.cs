using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class BlobTrigger1
    {
        private readonly ILogger<BlobTrigger1> _logger;
        private readonly ShareServiceClient _shareServiceClient;

        public BlobTrigger1(ShareServiceClient shareServiceClient, ILogger<BlobTrigger1> logger)
        {
            _logger = logger;
            _shareServiceClient = shareServiceClient;
        }

        [Function(nameof(BlobTrigger1))]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "c109c7_STORAGE")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
