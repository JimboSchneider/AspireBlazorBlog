using Microsoft.EntityFrameworkCore;
using AspireBlazorBlog.ApiService.Models;

namespace AspireBlazorBlog.ApiService.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure BlogPost
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Summary).HasMaxLength(500);
            
            entity.HasOne(e => e.Author)
                .WithMany(a => a.BlogPosts)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Bio).HasMaxLength(1000);
        });

        // Configure Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Configure Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.AuthorName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AuthorEmail).IsRequired().HasMaxLength(200);
            
            entity.HasOne(e => e.BlogPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure many-to-many relationship between BlogPost and Tag
        modelBuilder.Entity<BlogPost>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.BlogPosts)
            .UsingEntity<Dictionary<string, object>>(
                "BlogPostTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<BlogPost>().WithMany().HasForeignKey("BlogPostId"));

        // Seed initial data
        var authorId = Guid.NewGuid();
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = authorId,
                Name = "Admin",
                Email = "admin@aspireblog.com",
                Bio = "System administrator",
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // This allows us to handle both SQL Server and Cosmos DB configurations
        if (!optionsBuilder.IsConfigured)
        {
            // Configuration will be done via dependency injection
        }
    }
}