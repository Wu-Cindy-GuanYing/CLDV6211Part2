using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenueBookingSystem.Models;

namespace VenueBookingSystem.Controllers
{
    public class BookingController : Controller
    {

        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)

        {
            var booking = _context.Bookings
                .Include(i => i.Venue)
                .Include(i => i.Event)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                booking = booking.Where(i =>
                    i.Venue.VenueName.Contains(searchString) ||
                    i.Event.EventName.Contains(searchString));
            }

            return View(await booking.ToListAsync());
        }

        

        public IActionResult Create()
        {
            ViewData["Events"] = _context.Event.ToList();
            ViewData["Venues"] = _context.Venue.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            //check if an event exists already
            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

            // Checks if the same venue is hosting two events at the same time: "conflict"
            var conflict = await _context.Bookings
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueID == booking.VenueID &&
                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);
            //if eventdate and venueid is clashing = conflict
            if (conflict)//if clash: display message
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)//if not clashing: save new booking
            {
                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewData["Events"] = _context.Event.ToList();
                    ViewData["Venues"] = _context.Venue.ToList();
                    return View(booking);
                }
            }

            ViewData["Events"] = _context.Event.ToList();
            ViewData["Venues"] = _context.Venue.ToList();
            return View(booking);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(m => m.BookingID == id);


            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.VenueID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.VenueID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

public async Task<IActionResult> LogDate(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]

        public async Task<IActionResult> LogDate(int id, Booking model)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.BookingDate = model.BookingDate;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }
       

    }

}
