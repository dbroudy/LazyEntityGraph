using System.Data.Entity;
using LazyEntityGraph.EntityFramework.Tests.Model.TPC;
using LazyEntityGraph.EntityFramework.Tests.Model.TPH;
using LazyEntityGraph.EntityFramework.Tests.Model.TPT;

namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class BlogContext : DbContext
    {
        public BlogContext()
        {
        }

        public BlogContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Story> Stories { get; set; }

        public DbSet<BillingDetail> BillingDetails { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

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

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Localizations)
                .WithRequired(pl => pl.LocalizationFor)
                .HasForeignKey(pl => pl.PostId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Localizations)
                .WithRequired(cl => cl.LocalizationFor)
                .HasForeignKey(pl => pl.CategoryId);
        }
    }
}
