using LeagueFriendLadder.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueFriendLadder.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friend>().ToTable("friends");
            modelBuilder.Entity<Friend>().Property(f => f.Id).HasColumnName("id");

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Sender)
                .WithMany()
                .HasForeignKey(f => f.SenderUserId);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Receiver)
                .WithMany()
                .HasForeignKey(f => f.ReceiverUserId);

            modelBuilder.Entity<Friend>().Property(f => f.SenderUserId).HasColumnName("SenderUserId");
            modelBuilder.Entity<Friend>().Property(f => f.ReceiverUserId).HasColumnName("ReceiverUserId");
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