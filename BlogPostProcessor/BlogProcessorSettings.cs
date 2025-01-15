namespace BlogPostProcessor;

public class BlogProcessorSettings
{
    public string InputDirectory { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;

    public string MetadataDirectory { get; set; } = string.Empty;
    
    public string PostMetadataFile { get; set; } = string.Empty;
    public string InputFilePattern { get; set; } = "*.md";
}