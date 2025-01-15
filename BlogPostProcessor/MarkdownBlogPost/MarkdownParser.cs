using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace BlogPostProcessor.MarkdownBlogPost;

public class MarkdownParser
{
    private readonly MarkdownPipeline _pipeline;
    private readonly IDeserializer _deserializer;

    public MarkdownParser()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .Build();

        _deserializer = new DeserializerBuilder().Build();
    }

    public async Task<BlogPost> ParseBlogPostFileAsync(string filePath)
    {
        var markdown = await File.ReadAllTextAsync(filePath);
        return ParseBlogPost(markdown);
    }

    private BlogPost ParseBlogPost(string markdownText)
    {
        var document = Markdown.Parse(markdownText, _pipeline);
        var yamlBlock = document
            .Descendants<YamlFrontMatterBlock>()
            .FirstOrDefault();

        var blogPost = new BlogPost();

        if (yamlBlock != null)
        {
            var yaml = yamlBlock.Lines.ToString();
            var metadata = _deserializer.Deserialize<BlogPostMetadata>(yaml);
            blogPost.Metadata = metadata;
        }

        if (yamlBlock != null)
        {
            document.Remove(yamlBlock);
        }

        if (yamlBlock is { Line: 0 })
        {
            var contentStartPosition = yamlBlock.Lines.Count + 2;
            var allLines = markdownText.Split('\n');
            var contentLines = allLines.Skip(contentStartPosition);
            blogPost.Content = string.Join("\n", contentLines).Trim();
        }
        else
        {
            blogPost.Content = markdownText;
        }
        
        return blogPost;
    }
}