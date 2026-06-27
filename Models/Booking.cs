using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Your name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Guest Name")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        // Navigation property
        public Room? Room { get; set; }

        [NotMapped]
        public int Nights
        {
            get
            {
                var nights = (CheckOutDate.Date - BookingDate.Date).Days;
                return nights < 1 ? 1 : nights;
            }
        }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                if (Room == null)
                {
                    return 0m;
                }

                return Room.Price * Nights;
            }
        }
    }
}
