using AspireBlazorBlog.ApiService.Models;

namespace AspireBlazorBlog.ApiService.Services;

public interface IBlogPostService
{
    Task<IEnumerable<BlogPost>> GetPublishedPostsAsync();
    Task<BlogPost?> GetPostByIdAsync(Guid id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<BlogPost> CreatePostAsync(BlogPost blogPost);
    Task<BlogPost?> UpdatePostAsync(Guid id, BlogPost blogPost);
    Task<bool> DeletePostAsync(Guid id);
    Task<bool> PublishPostAsync(Guid id);
    Task<bool> UnpublishPostAsync(Guid id);
}