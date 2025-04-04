
public class AzureFileShareOptions
{
    public const string SectionName = "AzureFileShareOptions";
    public string Uri { get; set; } = string.Empty;
    public string ShareName { get; set; } = string.Empty;
    public string DirectoryName { get; set; } = string.Empty;
}