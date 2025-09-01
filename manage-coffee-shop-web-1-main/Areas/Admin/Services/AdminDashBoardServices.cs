using manage_coffee_shop_web.Areas.Admin.Models;
using manage_coffee_shop_web.Models;

namespace manage_coffee_shop_web.Areas.Admin.Services
{
    public class AdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public AdminDashboardViewModel GetDashboardData()
        {
            return new AdminDashboardViewModel
            {
                TotalBanners = _context.Banner.Count(),
                TotalCategories = _context.Category.Count(),
                TotalProducts = _context.Products.Count(),
                TotalUsers = _context.Users.Count()
            };
        }
    }
}