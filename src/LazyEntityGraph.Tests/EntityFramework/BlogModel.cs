using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace LazyEntityGraph.Tests.EntityFramework
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

    public class ContactDetails : Entity
    {
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
    }

    public class Tag : Entity
    {
        public string TagName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class BlogContext : DbContext
    {
        public BlogContext()
            : base("BlogContext")
        {

        }

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

            modelBuilder.Entity<ContactDetails>()
                .HasRequired(cd => cd.User)
                .WithOptional(u => u.ContactDetails);
        }
    }
}
