namespace Data.Interfaces;

public interface IBlogService
{
    Task<List<BlogPost>> GetAllPostsAsync();
    Task<int> GetBlogPostCountAsync();
    Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex);
    Task<BlogPost?> GetBlogPostAsync(string id);
}