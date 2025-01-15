using System.Net.Http.Json;
using Data.Interfaces;

namespace Data.Service;

public class BlogService : IBlogService
{
    private readonly HttpClient _http;
    private readonly Dictionary<string, BlogPost> _postCache = new();
    private SortedDictionary<string, PostMetadata>? _postMetadataCache;

    public BlogService(HttpClient http)
    {
        _http = http;
        _postMetadataCache = null;
    }

    private async Task<SortedDictionary<string, PostMetadata>> GetPostMetadata()
    {
        if (_postMetadataCache != null)
        {
            return _postMetadataCache;
        }

        var metadata = await _http.GetFromJsonAsync<Dictionary<string, PostMetadata>>("data/metadata/posts.json")
                       ?? new Dictionary<string, PostMetadata>();

        _postMetadataCache = new SortedDictionary<string, PostMetadata>(metadata, new PublishDateComparer(metadata));
        return _postMetadataCache;
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        var metadata = await GetPostMetadata();
        var posts = new List<BlogPost>();
        foreach (var postId in metadata.Keys)
        {
            var post = await GetBlogPostAsync(postId);
            if (post != null)
            {
                posts.Add(post);
            }
        }

        return posts;
    }

    public async Task<int> GetBlogPostCountAsync()
    {
        var metadata = await GetPostMetadata();
        return metadata.Count;
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        try
        {
            var metadata = await GetPostMetadata();

            var postIds = metadata.Keys
                .Skip(startIndex)
                .Take(numberOfPosts)
                .ToList();

            var postTasks = postIds.Select(GetBlogPostAsync);

            var posts = await Task.WhenAll(postTasks);

            return posts.Where(p => p != null).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        try
        {
            var metadata = await GetPostMetadata();

            if (!metadata.ContainsKey(id))
            {
                return null;
            }

            if (!_postCache.TryGetValue(id, out var postContent))
            {
                postContent = await _http.GetFromJsonAsync<BlogPost>($"data/posts/{id}.json");
                if (postContent != null)
                {
                    _postCache[id] = postContent;
                }
            }

            return postContent;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<BlogPost>> GetPostsByTags(IEnumerable<string> tags)
    {
        var metadata = await GetPostMetadata();
        var matchingPosts = new List<BlogPost>();
        var tagsSet = new HashSet<string>(tags);

        foreach (var postMetadata in metadata.Where(post => post.Value.Tags.Any(tag => tagsSet.Contains(tag))))
        {
            var fullPost = await GetBlogPostAsync(postMetadata.Key);
            if (fullPost != null)
            {
                matchingPosts.Add(fullPost);
            }
        }

        return matchingPosts.OrderByDescending(p => p.Metadata.PublishDate).ToList();
    }
}