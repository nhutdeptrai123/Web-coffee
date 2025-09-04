using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using manage_coffee_shop_web.Areas.Admin.Models;
using manage_coffee_shop_web.Areas.Admin.Services;

namespace manage_coffee_shop_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminDashBoardController : Controller
    {
        private readonly AdminDashboardService _dashboardService;

        public AdminDashBoardController(AdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var model = _dashboardService.GetDashboardData();
            return View(model);
        }
    }
}