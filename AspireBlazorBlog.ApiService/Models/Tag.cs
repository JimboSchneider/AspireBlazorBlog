namespace AspireBlazorBlog.ApiService.Models;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}