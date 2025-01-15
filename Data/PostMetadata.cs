namespace Data;

public class PostMetadata
{
    public string Title { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; } = DateTime.MinValue;
    public List<string> Tags { get; set; } = new List<string>();
}