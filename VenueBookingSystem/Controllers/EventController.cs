using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenueBookingSystem.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace VenueBookingSystem.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()

        {
            var Event = await _context.Event.ToListAsync();

            return View(Event);
        }

        public IActionResult Create()
        {
            ViewData["Venues"] = _context.Venue.ToList();
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event newevent)
        {
            if (ModelState.IsValid)
            {

                _context.Add(newevent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created succesfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venues"] = _context.Venue.ToList();
            return View(newevent);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var newevent = await _context.Event.FirstOrDefaultAsync(m => m.EventID == id);

            if (newevent == null)
            {
                return NotFound();
            }
            return View(newevent);
           
        }

        //step 1: confirm deletion (get)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var newevent = await _context.Event.FirstOrDefaultAsync(e => e.EventID == id);

            if (newevent == null)
            {
                return NotFound();
            }
            return View(newevent);
        }

        //step 2: perform deletion (POST): after user confirms to delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //checks if event exists using ID
            var newevent = await _context.Event.FindAsync(id);
            if (newevent == null)
            {
                return NotFound();
            }
            //if event has booking: cannot delete
            var isBooked = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings. ";
                return RedirectToAction(nameof(Index));
            }
            //if event has no booking: can delete event
            _context.Event.Remove(newevent);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted succesfully.";
            return RedirectToAction(nameof(Index));
        }


        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newevent = await _context.Event.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Venues"] = _context.Venue.ToList();
            return View(newevent);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event newevent)
        {
            if (id != newevent.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(newevent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated succesfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Venues"] = _context.Venue.ToList();
            return View(newevent);
        }


    }
}