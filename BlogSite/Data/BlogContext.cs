using BlogSite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogSite.Data;

public class BlogContext : IdentityDbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Comment> Comments { get; set; } // Add this line

}
