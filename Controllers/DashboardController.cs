using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var rooms = await _context.Rooms.ToListAsync();
            var bookings = await _context.Bookings.Include(b => b.Room).ToListAsync();

            var totalRooms = rooms.Count;
            var availableRooms = rooms.Count(r => r.IsAvailable);
            var totalBookings = bookings.Count();
            var occupancyRate = totalRooms == 0
                ? 0
                : (int)Math.Round(((double)(totalRooms - availableRooms) / totalRooms) * 100);

            var revenue = bookings.Sum(b => b.Room != null ? b.Room.Price * b.Nights : 0m);

            var recent = bookings
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                TotalBookings = totalBookings,
                OccupancyRate = occupancyRate,
                TotalRevenue = revenue,
                RecentBookings = recent
            };

            return View(model);
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UserDashboard()
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserName == userName)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var upcoming = bookings.Count(b => b.BookingDate.Date >= DateTime.Today);
            var totalSpend = bookings.Sum(b => b.Room != null ? b.Room.Price * b.Nights : 0m);

            var model = new UserDashboardViewModel
            {
                UserName = userName,
                TotalBookings = bookings.Count(),
                UpcomingBookings = upcoming,
                TotalSpend = totalSpend,
                RecentBookings = bookings.Take(5).ToList()
            };

            return View("User", model);
        }
    }
}
