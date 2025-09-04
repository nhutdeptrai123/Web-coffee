using manage_coffee_shop_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace manage_coffee_shop_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BannerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<BannerController> _logger;

        public BannerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, ILogger<BannerController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Admin/Banner
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Attempting to retrieve banners.");
            var banner = await _context.Banner.ToListAsync();
            _logger.LogInformation("Retrieved {Count} banners.", banner?.Count ?? 0);
            return View(banner);
        }

        // GET: Admin/Banner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Banner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Banner banner, IFormFile imageFile)
        {
            _logger.LogInformation("Creating new banner with Title: {Title}", banner.Title);
            if (imageFile != null && imageFile.Length > 0)
            {
                string folder = Path.Combine(_hostEnvironment.WebRootPath, "images/banners");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                banner.Image = "/images/banners/" + fileName;
            }

            _context.Add(banner);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Banner created with Id: {Id}", banner.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Banner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var banner = await _context.Banner.FindAsync(id);
            if (banner == null) return NotFound();

            return View(banner);
        }

        // POST: Admin/Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Banner banner, IFormFile imageFile)
        {
            if (id != banner.Id) return NotFound();
            try
            {
                var bannerDb = await _context.Banner.FindAsync(id);
                if (bannerDb == null) return NotFound();

                _logger.LogInformation("Updating banner with Id: {Id}", id);
                bannerDb.Title = banner.Title;
                bannerDb.Description = banner.Description;

                if (imageFile != null && imageFile.Length > 0)
                {
                    string folder = Path.Combine(_hostEnvironment.WebRootPath, "images/banners");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    bannerDb.Image = "/images/banners/" + fileName;
                }

                _context.Update(bannerDb);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Banner updated with Id: {Id}", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating banner with Id: {Id}", id);
                if (!_context.Banner.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Banner/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.Banner.FindAsync(id);
            if (banner == null) return NotFound();

            _logger.LogInformation("Deleting banner with Id: {Id}", id);
            if (!string.IsNullOrEmpty(banner.Image))
            {
                string filePath = Path.Combine(_hostEnvironment.WebRootPath, banner.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Banner.Remove(banner);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Banner deleted with Id: {Id}", id);
            return RedirectToAction(nameof(Index));
        }
    }
}