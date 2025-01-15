namespace BlogPostProcessor.Utilities;

public static class BlogPostUtility
{
    public static string GeneratePostId(this string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath)
            .ToLowerInvariant()
            .Replace(" ", "-");
    }
}