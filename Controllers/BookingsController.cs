using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: Bookings - All bookings (admin)
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
            return View(bookings);
        }

        [Authorize(Roles = "User,Admin")]
        // GET: Bookings/MyBookings - User views their bookings by name search
        public async Task<IActionResult> MyBookings(string? guestName)
        {
            var bookings = _context.Bookings.Include(b => b.Room).AsQueryable();

            if (User.IsInRole("Admin"))
            {
                if (!string.IsNullOrWhiteSpace(guestName))
                {
                    bookings = bookings.Where(b => b.UserName.Contains(guestName));
                }
            }
            else
            {
                var currentUser = User.Identity?.Name ?? string.Empty;
                bookings = bookings.Where(b => b.UserName == currentUser);
                guestName = currentUser;
            }

            ViewBag.GuestName = guestName;
            return View(await bookings.OrderByDescending(b => b.BookingDate).ToListAsync());
        }

        [Authorize(Roles = "User,Admin")]
        // GET: Bookings/Book/5
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null || !room.IsAvailable)
            {
                TempData["ErrorMessage"] = "This room is no longer available.";
                return RedirectToAction("Index", "Rooms");
            }

            var booking = new Booking
            {
                RoomId = room.Id,
                BookingDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1),
                UserName = User.Identity?.Name ?? string.Empty,
                Room = room
            };

            return View(booking);
        }

        [Authorize(Roles = "User,Admin")]
        // POST: Bookings/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book([Bind("RoomId,UserName,BookingDate,CheckOutDate")] Booking booking)
        {
            if (booking.CheckOutDate.Date <= booking.BookingDate.Date)
            {
                ModelState.AddModelError(nameof(booking.CheckOutDate), "Check-out must be after check-in.");
            }

            if (ModelState.IsValid)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    booking.UserName = User.Identity.Name ?? booking.UserName;
                }

                // Double-check availability
                var room = await _context.Rooms.FindAsync(booking.RoomId);
                if (room == null || !room.IsAvailable)
                {
                    TempData["ErrorMessage"] = "Sorry, this room was just booked by someone else.";
                    return RedirectToAction("Index", "Rooms");
                }

                // Save booking
                _context.Bookings.Add(booking);

                // Mark room unavailable
                room.IsAvailable = false;
                _context.Rooms.Update(room);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Booking confirmed! Reference ID: #{booking.Id}";
                return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
            }

            // Reload room for view
            booking.Room = await _context.Rooms.FindAsync(booking.RoomId);
            return View(booking);
        }

        [Authorize(Roles = "User,Admin")]
        // GET: Bookings/Confirmation/5
        public async Task<IActionResult> Confirmation(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [Authorize(Roles = "User,Admin")]
        // POST: Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? returnUrl)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var currentUser = User.Identity?.Name ?? string.Empty;
                if (!string.Equals(booking.UserName, currentUser, StringComparison.Ordinal))
                {
                    return Forbid();
                }
            }

            // Free up the room
            if (booking.Room != null)
            {
                booking.Room.IsAvailable = true;
                _context.Rooms.Update(booking.Room);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking cancelled successfully. The room is now available.";

            if (returnUrl == "admin")
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(MyBookings));
        }

        [Authorize(Roles = "User,Admin")]
        // GET: Bookings/Invoice/5
        public async Task<IActionResult> Invoice(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var currentUser = User.Identity?.Name ?? string.Empty;
                if (!string.Equals(booking.UserName, currentUser, StringComparison.Ordinal))
                {
                    return Forbid();
                }
            }

            return View(booking);
        }
    }
}
