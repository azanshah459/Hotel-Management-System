using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room Number is required")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room Type is required")]
        [Display(Name = "Room Type")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 999999, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price Per Night (PKR)")]
        public decimal Price { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
