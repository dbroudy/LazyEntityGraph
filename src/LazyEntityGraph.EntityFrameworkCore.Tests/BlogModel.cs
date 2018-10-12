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

    public class Post : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }

        public int PosterId { get; set; }
        public virtual User Poster { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
   
    public class Story : Post
    {
        
    }

    public class Category : Entity
    {
        public string CategoryName { get; set; }
    }

    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
                : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Story> Stories { get; set; }

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
        }
    }
}
