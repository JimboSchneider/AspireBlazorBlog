namespace AspireBlazorBlog.ApiService.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}