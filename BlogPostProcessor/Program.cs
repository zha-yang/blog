using BlogPostProcessor.MarkdownBlogPost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BlogPostProcessor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation) 
                                    ?? throw new DirectoryNotFoundException("Could not determine assembly directory");

            var settingsPath = FindSettingsFile(assemblyDirectory);
            var projectDirectory = Path.GetDirectoryName(settingsPath)!;

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(projectDirectory)  // Use the project directory
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            
            var settings = new BlogProcessorSettings();
            configuration.GetSection("BlogProcessorSettings").Bind(settings);
            
            var markdownParser = new MarkdownParser();
            var processor = new BlogPostProcessor(markdownParser, settings);
            await processor.ProcessPosts();
            Console.WriteLine("Blog posts processed successfully");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing blog posts: {ex.Message}");
            return 1;
        }
    }
    
    private static string FindSettingsFile(string startPath)
    {
        var currentDirectory = new DirectoryInfo(startPath);
        
        for (int i = 0; i < 4 && currentDirectory != null; i++)
        {
            var settingsPath = Path.Combine(currentDirectory.FullName, "appsettings.json");
            if (File.Exists(settingsPath))
            {
                return settingsPath;
            }
            currentDirectory = currentDirectory.Parent;
        }
    
        throw new FileNotFoundException(
            $"Could not find appsettings.json in or above: {startPath}");
    }
}