using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms - Show only available rooms to users
        public async Task<IActionResult> Index(string? type, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            var query = _context.Rooms.Where(r => r.IsAvailable).AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(r => r.Type == type);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= maxPrice.Value);
            }

            query = sort switch
            {
                "price-asc" => query.OrderBy(r => r.Price),
                "price-desc" => query.OrderByDescending(r => r.Price),
                _ => query.OrderBy(r => r.RoomNumber)
            };

            ViewBag.RoomTypes = await _context.Rooms
                .Select(r => r.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.SelectedType = type;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;

            return View(await query.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
            if (room == null) return NotFound();

            return View(room);
        }

        // ── ADMIN ACTIONS ──────────────────────────────────────────────

        [Authorize(Roles = "Admin")]
        // GET: Rooms/AdminIndex - All rooms for admin
        public async Task<IActionResult> AdminIndex()
        {
            var allRooms = await _context.Rooms.ToListAsync();
            return View(allRooms);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomNumber,Type,Price")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.IsAvailable = true;
                _context.Add(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room {room.RoomNumber} created successfully!";
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomNumber,Type,Price,IsAvailable")] Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Room {room.RoomNumber} updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(e => e.Id == room.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
            if (room == null) return NotFound();

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                // Also remove related bookings
                var bookings = _context.Bookings.Where(b => b.RoomId == id);
                _context.Bookings.RemoveRange(bookings);
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room deleted successfully!";
            }
            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
