using Microsoft.AspNetCore.Mvc;
using AspireBlazorBlog.ApiService.Models;
using AspireBlazorBlog.ApiService.Services;

namespace AspireBlazorBlog.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<BlogPostsController> _logger;

    public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
    {
        _blogPostService = blogPostService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
    {
        var posts = await _blogPostService.GetPublishedPostsAsync();
        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BlogPost>> GetBlogPost(Guid id)
    {
        var blogPost = await _blogPostService.GetPostByIdAsync(id);
        if (blogPost == null)
        {
            return NotFound();
        }
        return Ok(blogPost);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<BlogPost>> GetBlogPostBySlug(string slug)
    {
        var blogPost = await _blogPostService.GetPostBySlugAsync(slug);
        if (blogPost == null)
        {
            return NotFound();
        }
        return Ok(blogPost);
    }

    [HttpPost]
    public async Task<ActionResult<BlogPost>> CreateBlogPost(CreateBlogPostRequest request)
    {
        var blogPost = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary,
            AuthorId = request.AuthorId,
            IsPublished = false
        };

        var createdPost = await _blogPostService.CreatePostAsync(blogPost);
        return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.Id }, createdPost);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BlogPost>> UpdateBlogPost(Guid id, UpdateBlogPostRequest request)
    {
        var blogPost = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary
        };

        var updatedPost = await _blogPostService.UpdatePostAsync(id, blogPost);
        if (updatedPost == null)
        {
            return NotFound();
        }

        return Ok(updatedPost);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
        var deleted = await _blogPostService.DeletePostAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishBlogPost(Guid id)
    {
        var published = await _blogPostService.PublishPostAsync(id);
        if (!published)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> UnpublishBlogPost(Guid id)
    {
        var unpublished = await _blogPostService.UnpublishPostAsync(id);
        if (!unpublished)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public record CreateBlogPostRequest(string Title, string Content, string Summary, Guid AuthorId);
public record UpdateBlogPostRequest(string Title, string Content, string Summary);