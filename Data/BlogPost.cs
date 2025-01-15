namespace Data;

public class BlogPost
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public PostMetadata Metadata { get; set; } = new PostMetadata();
}