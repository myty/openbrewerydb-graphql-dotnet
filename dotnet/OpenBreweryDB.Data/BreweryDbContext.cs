using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Data.Models.Favorites;
using OpenBreweryDB.Data.Models.Reviews;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.Data
{
    public class BreweryDbContext : DbContext
    {
        public BreweryDbContext(DbContextOptions<BreweryDbContext> options) : base(options) { }

        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<BreweryTag> BreweryTags { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity: Brewery
            modelBuilder.Entity<Brewery>()
                .ToTable("breweries");

            modelBuilder.Entity<Brewery>()
                .Property(b => b.Latitude)
                .HasPrecision(8, 6);

            modelBuilder.Entity<Brewery>()
                .Property(b => b.Longitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Brewery>()
                .HasMany(b => b.BreweryReviews)
                .WithOne(r => r.Brewery);

            // Entity: BreweryTag
            modelBuilder.Entity<BreweryTag>()
                .HasKey(t => new { t.BreweryId, t.TagId });

            modelBuilder.Entity<BreweryTag>()
                .HasOne(bt => bt.Brewery)
                .WithMany(b => b.BreweryTags)
                .HasForeignKey(bt => bt.BreweryId);

            modelBuilder.Entity<BreweryTag>()
                .HasOne(bt => bt.Tag)
                .WithMany(t => t.BreweryTags)
                .HasForeignKey(bt => bt.TagId);

            // Entity: Tag
            var tagEntityBuilder = modelBuilder.Entity<Tag>()
                .ToTable("tags");
            tagEntityBuilder
                .HasIndex(b => b.Name)
                .HasDatabaseName("Index_Tags_On_Name");

            // Entity: User
            modelBuilder.Entity<User>()
                .ToTable("users");

            // Entity: Favorite
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.BreweryId, f.UserId });
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Brewery)
                .WithMany(u => u.FavoriteUsers)
                .HasForeignKey(u => u.BreweryId);

            // Entity: Review
            modelBuilder.Entity<Review>()
                .ToTable("reviews");
        }
    }
}
