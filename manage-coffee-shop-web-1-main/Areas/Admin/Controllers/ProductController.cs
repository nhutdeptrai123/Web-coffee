using manage_coffee_shop_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace manage_coffee_shop_web.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile ImageFile)
        {
            // Thêm kiểm tra validation để xử lý lỗi khi form trống hoặc dữ liệu không hợp lệ
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, load lại SelectList và trả về View
                ViewBag.CategoryId = new SelectList(_context.Category, "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                model.Image = "/images/products/" + fileName;
            }
            else
            {
                // Thêm lỗi nếu không có ảnh được tải lên (nếu ảnh là trường bắt buộc)
                ModelState.AddModelError("Image", "Vui lòng tải lên một hình ảnh.");
                ViewBag.CategoryId = new SelectList(_context.Category, "Id", "Name", model.CategoryId);
                return View(model);
            }

            model.CreatedDate = DateTime.Now;
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Product model, IFormFile ImageFile)
{
    if (id != model.Id)
    {
        return NotFound();
    }

    // Lấy sản phẩm từ database và theo dõi nó
    var productToUpdate = await _context.Products.FindAsync(id);
    if (productToUpdate == null)
    {
        return NotFound();
    }

    // Cập nhật các trường từ model được gửi lên
    productToUpdate.Name = model.Name;
    productToUpdate.Description = model.Description;
    productToUpdate.Price = model.Price;
    productToUpdate.CategoryId = model.CategoryId;
    // CreatedDate và ViewCount không cần cập nhật vì chúng không đổi

    // Xử lý tải ảnh mới
    if (ImageFile != null && ImageFile.Length > 0)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

        // Xóa ảnh cũ nếu có
        if (!string.IsNullOrEmpty(productToUpdate.Image))
        {
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productToUpdate.Image.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ImageFile.CopyToAsync(stream);
        }
        productToUpdate.Image = "/images/products/" + fileName;
    }

    // Kiểm tra validation một lần nữa sau khi đã xử lý ảnh
    if (!ModelState.IsValid)
    {
        ViewBag.CategoryId = new SelectList(_context.Category, "Id", "Name", model.CategoryId);
        return View(model);
    }

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ProductExists(model.Id))
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

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Xóa ảnh khi xóa sản phẩm
                if (!string.IsNullOrEmpty(product.Image))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.Image.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}