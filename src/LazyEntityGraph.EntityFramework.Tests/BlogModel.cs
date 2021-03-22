using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace LazyEntityGraph.EntityFramework.Tests
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }

    public class User : Entity
    {
        public string Username { get; set; }

        public virtual ContactDetails ContactDetails { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public int DefaultCategoryId { get; set; }
        public virtual Category DefaultCategory { get; set; }
    }

    public class ContactDetails
    {
        [Key, ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Post : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }

        public int PosterId { get; set; }
        public virtual User Poster { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }

    public class Story : Post
    {

    }

    public class Tag : Entity
    {
        public string TagName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Category : Entity
    {
        public string CategoryName { get; set; }
    }

    public class BlogContext : DbContext
    {
        public BlogContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Posts);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithRequired(p => p.Poster)
                .HasForeignKey(p => p.PosterId);

            modelBuilder.Entity<User>()
                .HasRequired(u => u.DefaultCategory)
                .WithMany()
                .HasForeignKey(u => u.DefaultCategoryId);

            modelBuilder.Entity<ContactDetails>()
                .HasRequired(cd => cd.User)
                .WithOptional(u => u.ContactDetails);
        }
    }
}
