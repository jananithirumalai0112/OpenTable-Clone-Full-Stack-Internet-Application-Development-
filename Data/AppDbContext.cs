using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Models;

namespace OpenTableApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Metropolis> Metropolises { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Reservation → User relationship
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Metropolis
            modelBuilder.Entity<Metropolis>().HasData(
                new Metropolis { Id = 1, Name = "New York", State = "NY", Country = "USA" },
                new Metropolis { Id = 2, Name = "Los Angeles", State = "CA", Country = "USA" }
            );

            // Seed Restaurant
            modelBuilder.Entity<Restaurant>().HasData(
                new Restaurant
                {
                    Id = 1,
                    Name = "Joe's Diner",
                    Address = "123 Main St",
                    Phone = "123-456-7890",
                    OpenHours = "11:00-14:00,17:00-21:00",
                    ImagePath = "img/joes.jpg",
                    MetropolisId = 1,
                    PriceRange = PriceRange.DoubleDollar,
                    CuisineStyle = "American"
                }
            );
        }
    }
}
