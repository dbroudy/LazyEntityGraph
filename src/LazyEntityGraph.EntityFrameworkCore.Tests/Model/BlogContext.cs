using LazyEntityGraph.EntityFrameworkCore.Tests.Model.TPH;
using Microsoft.EntityFrameworkCore;

namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
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