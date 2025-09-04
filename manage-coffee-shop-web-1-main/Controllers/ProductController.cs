using Microsoft.AspNetCore.Mvc;
using manage_coffee_shop_web.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ProductController : Controller {
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}