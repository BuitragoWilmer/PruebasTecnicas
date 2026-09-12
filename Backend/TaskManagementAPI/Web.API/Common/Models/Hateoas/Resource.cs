namespace Web.API.Common.Models.Hateoas;

public class Resource<T>
{
    public Resource(T data, List<Link> links)
    {
        Data = data;
        Links = links;
    }

    public T Data { get; set; }
    public List<Link> Links { get; set; } = new();
}

public class ResourceCollection<T>
{
    public IReadOnlyList<T> Items { get; }
    public List<Link> Links { get; }

    public ResourceCollection(IReadOnlyList<T> items, List<Link> links)
    {
        Items = items;
        Links = links;
    }
}