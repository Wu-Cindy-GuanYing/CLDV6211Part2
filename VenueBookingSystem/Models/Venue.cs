using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //import to add
using Microsoft.AspNetCore.Http;// important to add

namespace VenueBookingSystem.Models
{
    public class Venue
    {
        
        public int VenueID { get; set; }

        [Required]
        public string VenueName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        //stays the same in order to store the image url to upload
        public string ImageURL { get; set; } 

        [NotMapped]//for uploading from the create/edit form
        public IFormFile? ImageFile { get; set; }
        public List<Booking> Bookings { get; set; } = new();

    }
}
