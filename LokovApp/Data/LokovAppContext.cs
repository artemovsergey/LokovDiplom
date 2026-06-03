using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Data
{
    public class LokovAppContext : DbContext
    {
        public LokovAppContext(DbContextOptions<LokovAppContext> options)
            : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().HasIndex(c => c.Phone).IsUnique();

            modelBuilder
                .Entity<Project>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Comment>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
