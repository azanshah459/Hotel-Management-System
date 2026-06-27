using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed sample rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, RoomNumber = "101", Type = "Standard", Price = 5000, IsAvailable = true },
                new Room { Id = 2, RoomNumber = "102", Type = "Deluxe", Price = 8500, IsAvailable = true },
                new Room { Id = 3, RoomNumber = "201", Type = "Suite", Price = 15000, IsAvailable = true },
                new Room { Id = 4, RoomNumber = "202", Type = "Standard", Price = 5000, IsAvailable = true },
                new Room { Id = 5, RoomNumber = "301", Type = "Presidential Suite", Price = 35000, IsAvailable = true }
            );

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.Role).HasMaxLength(10);
            });
        }
    }
}
