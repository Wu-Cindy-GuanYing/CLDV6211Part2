using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenueBookingSystem.Models;

namespace VenueBookingSystem.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()

        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {


                // Handle image upload to Azure Blob Storage if an image file was provided
                // This is Step 4C: Modify Controller to receive ImageFile from View (user upload)
                // This is Step 5: Upload selected image to Azure Blob Storage
                if (venue.ImageFile != null)
                {

                    // Upload image to Blob Storage (Azure)
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile); //Main part of Step 5 B (upload image to Azure Blob Storage)

                    // Step 6: Save the Blob URL into ImageUrl property (the database)
                    venue.ImageURL = blobUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Model error in {modelState.Key}: {error.ErrorMessage}");
                }
            }

            return View(venue);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(v => v.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        //Perform Deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it has existing booking.";
                return RedirectToAction(nameof(Index));
            }
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        // Upload new image if provided
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        // STep 6
                        // Update Venue.ImageUrl with new Blob URL
                        venue.ImageURL = blobUrl;
                    }
                    else
                    {
                        // Keep the existing ImageUrl (Optional depending on your UI design)
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        // This is Step 5 (C): Upload selected image to Azure Blob Storage.
        // It completes the entire uploading process inside Step 5 �� from connecting to Azure to returning the Blob URL after upload.
        // This will upload the Image to Blob Storage Account
        // Uploads an image to Azure Blob Storage and returns the Blob URL
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "Replace";
            var containerName = "venue01";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }
        public async Task<IActionResult> Details(int? id)
        {

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }
    }
}