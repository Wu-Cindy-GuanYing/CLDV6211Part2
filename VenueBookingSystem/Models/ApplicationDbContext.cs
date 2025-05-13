using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace VenueBookingSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
