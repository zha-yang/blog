using System.Net.Http.Json;
using Data.Interfaces;

namespace Data.Service;

public class BlogService : IBlogService
{
    private readonly HttpClient _http;
    private Dictionary<string, BlogPost> _postCache = new();
    private Dictionary<string, PostMetadata> _postMetadataMap = new();

    public BlogService(HttpClient http)
    {
        _http = http;
    }

    public async Task Initialize()
    {
        _postCache.Clear();
        _postMetadataMap.Clear();
        _postMetadataMap =
            await _http.GetFromJsonAsync<Dictionary<string, PostMetadata>>("data/metadata/posts.json") ??
            new Dictionary<string, PostMetadata>();
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        var posts = new List<BlogPost>();
        foreach (var postId in _postMetadataMap.Keys)
        {
            if (!_postCache.TryGetValue(postId, out var value))
            {
                value = await _http.GetFromJsonAsync<BlogPost>($"data/posts/{postId}.json");
                if (value != null)
                {
                    _postCache[postId] = value;
                }
            }

            if (value != null)
            {
                posts.Add(value);
            }
        }

        return posts;
    }

    public Task<int> GetBlogPostCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        throw new NotImplementedException();
    }

    public Task<BlogPost?> GetBlogPostAsync(string id)
    {
        throw new NotImplementedException();
    }

    private async Task<BlogPost?> GetPostAsync(string id)
    {
        if (!_postCache.TryGetValue(id, out BlogPost? value))
        {
            value = await _http.GetFromJsonAsync<BlogPost>($"data/posts/{id}.json");
            if (value != null)
            {
                _postCache[id] = value;
            }
        }

        return value;
    }
}