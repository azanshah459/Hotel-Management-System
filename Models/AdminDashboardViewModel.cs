using HotelManagement.Models;

namespace HotelManagement.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalBookings { get; set; }
        public int OccupancyRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
    }
}
