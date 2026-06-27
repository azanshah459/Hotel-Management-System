using HotelManagement.Models;

namespace HotelManagement.Models
{
    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public decimal TotalSpend { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
    }
}
