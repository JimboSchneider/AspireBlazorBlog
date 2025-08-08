using AspireBlazorBlog.ApiService.Data;
using AspireBlazorBlog.ApiService.Models;
using AspireBlazorBlog.ApiService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AspireBlazorBlog.Tests;

public class BlogPostServiceTests
{
    [Fact]
    public async Task UpdatePostAsync_RegeneratesSlug_WhenTitleChanges()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new BlogDbContext(options);
        var logger = new LoggerFactory().CreateLogger<BlogPostService>();
        var service = new BlogPostService(context, logger);

        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            Content = "Content",
            Summary = "Summary",
            Slug = "original-title",
            CreatedAt = DateTime.UtcNow
        };
        context.BlogPosts.Add(post);
        await context.SaveChangesAsync();

        var update = new BlogPost
        {
            Title = "New Title",
            Content = "Content",
            Summary = "Summary"
        };

        // Act
        var result = await service.UpdatePostAsync(post.Id, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-title", result!.Slug);
    }
}

