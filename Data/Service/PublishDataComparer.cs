namespace Data.Service;

public class PublishDateComparer : IComparer<string>
{
    private readonly Dictionary<string, PostMetadata> _metadata;

    public PublishDateComparer(Dictionary<string, PostMetadata> metadata)
    {
        _metadata = metadata;
    }

    public int Compare(string x, string y)
    {
        return DateTime.Compare(_metadata[y].PublishDate, _metadata[x].PublishDate);
    }
}