namespace BlogPostProcessor.MarkdownBlogPost;

public class BlogPost
{
    public BlogPostMetadata Metadata { get; set; } = new BlogPostMetadata();
    public string Content { get; set; } = string.Empty;
}