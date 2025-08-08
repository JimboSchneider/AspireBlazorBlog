using Microsoft.EntityFrameworkCore;
using AspireBlazorBlog.ApiService.Data;
using AspireBlazorBlog.ApiService.Models;

namespace AspireBlazorBlog.ApiService.Services;

public class BlogPostService : IBlogPostService
{
    private readonly BlogDbContext _context;
    private readonly ILogger<BlogPostService> _logger;

    public BlogPostService(BlogDbContext context, ILogger<BlogPostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
    {
        return await _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetPostByIdAsync(Guid id)
    {
        return await _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        return await _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<BlogPost> CreatePostAsync(BlogPost blogPost)
    {
        // Set server-generated values
        blogPost.Id = Guid.NewGuid();
        blogPost.CreatedAt = DateTime.UtcNow;
        blogPost.Slug = GenerateSlug(blogPost.Title);

        // Ensure the slug is unique
        var slugBase = blogPost.Slug;
        var counter = 1;
        while (await _context.BlogPosts.AnyAsync(p => p.Slug == blogPost.Slug))
        {
            blogPost.Slug = $"{slugBase}-{counter}";
            counter++;
        }

        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new blog post with ID {BlogPostId}", blogPost.Id);
        return blogPost;
    }

    public async Task<BlogPost?> UpdatePostAsync(Guid id, BlogPost blogPost)
    {
        var existingPost = await _context.BlogPosts.FindAsync(id);
        if (existingPost == null)
        {
            return null;
        }

        // Check if the title is actually changing before we overwrite it
        var titleChanged = existingPost.Title != blogPost.Title;

        // Update only allowed fields
        existingPost.Title = blogPost.Title;
        existingPost.Content = blogPost.Content;
        existingPost.Summary = blogPost.Summary;
        existingPost.UpdatedAt = DateTime.UtcNow;

        // Regenerate slug if title changed
        if (titleChanged)
        {
            existingPost.Slug = GenerateSlug(blogPost.Title);

            // Ensure uniqueness
            var slugBase = existingPost.Slug;
            var counter = 1;
            while (await _context.BlogPosts.AnyAsync(p => p.Slug == existingPost.Slug && p.Id != id))
            {
                existingPost.Slug = $"{slugBase}-{counter}";
                counter++;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated blog post with ID {BlogPostId}", id);
        return existingPost;
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Deleted blog post with ID {BlogPostId}", id);
        return true;
    }

    public async Task<bool> PublishPostAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        blogPost.IsPublished = true;
        blogPost.PublishedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Published blog post with ID {BlogPostId}", id);
        return true;
    }

    public async Task<bool> UnpublishPostAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        blogPost.IsPublished = false;
        blogPost.PublishedAt = null;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Unpublished blog post with ID {BlogPostId}", id);
        return true;
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("&", "and")
            .Replace("+", "plus");
    }
}