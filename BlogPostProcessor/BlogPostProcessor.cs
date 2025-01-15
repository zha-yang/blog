using BlogPostProcessor.MarkdownBlogPost;
using BlogPostProcessor.Utilities;
using Data;
using Newtonsoft.Json;

namespace BlogPostProcessor;

public class BlogPostProcessor
{
    private readonly MarkdownParser _markdownParser;
    private readonly BlogProcessorSettings _settings;

    public BlogPostProcessor(MarkdownParser markdownParser, BlogProcessorSettings settings)
    {
        _markdownParser = markdownParser;
        _settings = settings;
    }

    public async Task ProcessPosts()
    {
        // Clean up before processing
        CleanupDirectory(_settings.OutputDirectory);
        CleanupDirectory(_settings.MetadataDirectory);

        // Ensure directories exist
        Directory.CreateDirectory(_settings.InputDirectory);
        Directory.CreateDirectory(_settings.OutputDirectory);
        Directory.CreateDirectory(_settings.MetadataDirectory);

        var postPaths = Directory.GetFiles(
            _settings.InputDirectory,
            _settings.InputFilePattern);

        var metadataList = new Dictionary<string, PostMetadata>();

        foreach (var postPath in postPaths)
        {
            var post = await _markdownParser.ParseBlogPostFileAsync(postPath);
            var metadata = new PostMetadata
            {
                Title = post.Metadata.Title,
                PublishDate = post.Metadata.LastModifiedDate,
                Tags = post.Metadata.Tags,
            };
            var postId = metadata.Title.GeneratePostId();
            
            metadataList.Add(postId, metadata);
            
            var postJsonFormat = new Data.BlogPost
            {
                Id = postId,
                Metadata = metadata,
                Content = post.Content
            };

            var outputPath = Path.Combine(
                _settings.OutputDirectory,
                $"{postJsonFormat.Id}.json");

            await File.WriteAllTextAsync(outputPath,
                JsonConvert.SerializeObject(postJsonFormat));
        }
        
        var metadataPath = Path.Combine(_settings.MetadataDirectory, "posts.json");
        await File.WriteAllTextAsync(metadataPath, JsonConvert.SerializeObject(metadataList));
    }

    private void CleanupDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            var files = Directory.GetFiles(directory, "*.json");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
        else
        {
            Directory.CreateDirectory(directory);
        }
    }
}