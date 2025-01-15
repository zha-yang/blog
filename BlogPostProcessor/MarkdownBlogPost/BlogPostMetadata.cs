using YamlDotNet.Serialization;
namespace BlogPostProcessor.MarkdownBlogPost;

public class BlogPostMetadata
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; } = string.Empty;
    
    [YamlMember(Alias = "date")]
    public DateTime LastModifiedDate { get; set; }
    
    [YamlMember(Alias = "tags")]
    public List<string> Tags { get; set; } = new List<string>();
}