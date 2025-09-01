using Microsoft.AspNetCore.Mvc;

namespace manage_coffee_shop_web.Areas.Admin.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
