using Azure.Identity;
using Azure.Storage.DataMovement;
using Azure.Storage.Files.Shares;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();
builder.Services.AddSingleton(new TransferManager(new TransferManagerOptions
{
    MaximumConcurrency = 10,
}));

builder.Services.AddOptions<AzureFileShareOptions>()
    .BindConfiguration(AzureFileShareOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// https://learn.microsoft.com/en-us/azure/storage/files/authorize-data-operations-portal#authenticate-with-your-microsoft-entra-account
// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/storage/Azure.Storage.DataMovement.Files.Shares#permissions
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<AzureFileShareOptions>>().Value;
    var uri = new Uri(options.Uri);
    var credential = new DefaultAzureCredential();

    // This registers a base ShareServiceClient without specifying a share name or directory name.
    // If the share name is known at compile time (in this case it's an optional value in AzureFileShareOptions), you can use the following code instead:
    // return new ShareFileClient(uri, credential);

    var client = new ShareServiceClient(uri, credential) ;
    return client.GenerateAccountSasUri()
        .GetDirectoryClient(options.DirectoryName);
});



builder.Build().Run();
