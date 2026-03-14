using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.DAL;

public class ForumDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    /// <summary>Forum users (display names, etc.). Identity users are in AspNetUsers via base.Users.</summary>
    public new DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionTag> QuestionTags => Set<QuestionTag>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Identity (AspNetUsers, AspNetRoles, etc.)

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.UserId);
            b.Property(u => u.Name).IsRequired().HasMaxLength(100);
            b.Property(u => u.Email).IsRequired().HasMaxLength(200);
            b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(c => c.CategoryId);
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.Property(c => c.Slug).IsRequired().HasMaxLength(150);
            b.HasIndex(c => c.Slug).IsUnique();
        });

        modelBuilder.Entity<Tag>(b =>
        {
            b.HasKey(t => t.TagId);
            b.Property(t => t.Name).IsRequired().HasMaxLength(100);
            b.Property(t => t.Slug).IsRequired().HasMaxLength(150);
            b.HasIndex(t => t.Slug).IsUnique();
        });

        modelBuilder.Entity<Question>(b =>
        {
            b.HasKey(q => q.QuestionId);
            b.Property(q => q.Title).IsRequired().HasMaxLength(200);
            b.Property(q => q.Description).IsRequired();

            b.HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(q => q.Category)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(q => q.CategoryId);
        });

        modelBuilder.Entity<QuestionTag>(b =>
        {
            b.HasKey(qt => qt.QuestionTagId);

            b.HasOne(qt => qt.Question)
                .WithMany(q => q.QuestionTags)
                .HasForeignKey(qt => qt.QuestionId);

            b.HasOne(qt => qt.Tag)
                .WithMany(t => t.QuestionTags)
                .HasForeignKey(qt => qt.TagId);

            b.HasIndex(qt => qt.QuestionId);
            b.HasIndex(qt => qt.TagId);
        });

        modelBuilder.Entity<Answer>(b =>
        {
            b.HasKey(a => a.AnswerId);
            b.Property(a => a.Content).IsRequired();

            b.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);

            b.HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(b =>
        {
            b.HasKey(c => c.CommentId);
            b.Property(c => c.Content).IsRequired();

            b.HasOne(c => c.Answer)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AnswerId);

            b.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(c => c.ParentComment)
                .WithMany(pc => pc.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vote>(b =>
        {
            b.HasKey(v => v.VoteId);

            b.HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(v => v.Question)
                .WithMany(q => q.Votes)
                .HasForeignKey(v => v.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(v => v.Answer)
                .WithMany(a => a.Votes)
                .HasForeignKey(v => v.AnswerId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(v => v.Comment)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(v => new { v.UserId, v.QuestionId }).IsUnique().HasFilter("[QuestionId] IS NOT NULL");
            b.HasIndex(v => new { v.UserId, v.AnswerId }).IsUnique().HasFilter("[AnswerId] IS NOT NULL");
            b.HasIndex(v => new { v.UserId, v.CommentId }).IsUnique().HasFilter("[CommentId] IS NOT NULL");
        });
    }
}

