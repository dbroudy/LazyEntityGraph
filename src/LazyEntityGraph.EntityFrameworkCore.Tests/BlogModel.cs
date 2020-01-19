using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFrameworkCore.Tests
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }

    public abstract class Localization : Entity
    {
        public string LanguageCode { get; set; }
    }
    
    public abstract class LocalizedEntity<T> : Entity
        where T : Localization
    {
        public virtual ICollection<T> Localizations { get; set; }
    }

    public class User : Entity
    {
        public string Username { get; set; }

        public virtual ContactDetails ContactDetails { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class ContactDetails
    {
        [Key]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Post : LocalizedEntity<PostLocalized>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }

        public int PosterId { get; set; }
        public virtual User Poster { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }

    public class PostLocalized : Localization
    {
        [ForeignKey(nameof(LocalizationFor))]
        public int PostId { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public virtual Post LocalizationFor { get; set; }
    }
   
    public class Story : Post
    {
        
    }

    public class Category : LocalizedEntity<CategoryLocalized>
    {
        public string CategoryName { get; set; }
    }

    public class CategoryLocalized : Localization
    {
        [ForeignKey(nameof(LocalizationFor))]
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public virtual Category LocalizationFor { get; set; }
    }

    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
                : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<PostLocalized> PostsLocalized { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Poster).IsRequired()
                .HasForeignKey(p => p.PosterId);

            modelBuilder.Entity<ContactDetails>()
                .HasOne(cd => cd.User)
                .WithOne(u => u.ContactDetails)
                .HasForeignKey<ContactDetails>(cd => cd.UserId);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Localizations)
                .WithOne(pl => pl.LocalizationFor).IsRequired()
                .HasForeignKey(pl => pl.PostId);
            
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Localizations)
                .WithOne(cl => cl.LocalizationFor).IsRequired()
                .HasForeignKey(pl => pl.CategoryId);
            
            modelBuilder.Entity<PostLocalized>()
                .HasAlternateKey(
                    nameof(Post.Id), 
                    nameof(PostLocalized.LanguageCode));
            
            modelBuilder.Entity<CategoryLocalized>()
                .HasAlternateKey(
                    nameof(Category.Id), 
                    nameof(CategoryLocalized.LanguageCode));
        }
    }
}
