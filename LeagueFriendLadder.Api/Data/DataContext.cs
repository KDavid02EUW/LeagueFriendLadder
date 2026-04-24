using Microsoft.EntityFrameworkCore;
using LeagueFriendLadder.Api.Models;

namespace LeagueFriendLadder.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("name");
                entity.Property(e => e.PasswordHash).HasColumnName("password");
                entity.Property(e => e.IsAdmin).HasColumnName("admin");

                entity.Property(e => e.Summoners)
                    .HasColumnName("summoners")
                    .HasColumnType("text[]");
            });
        }
    }
}