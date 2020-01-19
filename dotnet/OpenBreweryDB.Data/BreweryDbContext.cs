using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Data
{
    public class BreweryDbContext : DbContext
    {
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<BreweryTag> BreweryTags { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=openbrewery.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity: Brewery
            modelBuilder.Entity<Brewery>()
                .ToTable("breweries");

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
            var tagEnityBuilder = modelBuilder.Entity<Tag>()
                .ToTable("tags");
            tagEnityBuilder
                .HasIndex(b => b.Name)
                .HasName("Index_Tags_On_Name");
        }
    }
}