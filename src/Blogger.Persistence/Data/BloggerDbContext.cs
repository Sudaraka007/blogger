using Blogger.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Persistence.Data;

public class BloggerDbContext(DbContextOptions<BloggerDbContext> options) : DbContext(options)
{
    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("author");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Surname)
                .HasColumnName("surname")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Removed)
                .HasColumnName("removed")
                .HasDefaultValue(false);

            entity.HasMany(e => e.Posts)
                .WithOne(e => e.Author)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("post");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(e => e.Content)
                .HasColumnName("content");

            entity.Property(e => e.Removed)
                .HasColumnName("removed")
                .HasDefaultValue(false);

            entity.HasIndex(e => e.AuthorId);
        });
    }
}
