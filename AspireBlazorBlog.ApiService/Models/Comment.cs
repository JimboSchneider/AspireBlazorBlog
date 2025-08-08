namespace AspireBlazorBlog.ApiService.Models;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }
    public Guid BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;
}