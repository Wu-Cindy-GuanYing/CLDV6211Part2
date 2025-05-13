using System.ComponentModel.DataAnnotations;

namespace VenueBookingSystem.Models;

public class Event
{
    public int EventID { get; set; }
    [Required]
    public string EventName { get; set; }

    [Required]
    public DateTime EventDate { get; set; }
    public string? Description { get; set; }

    public int? VenueID { get; set; }
    //public Venue? Venues { get; set; }
    public List<Booking> Bookings { get; set; } = new();

}
