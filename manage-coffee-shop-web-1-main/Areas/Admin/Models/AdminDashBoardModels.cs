using System;

namespace manage_coffee_shop_web.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalBanners { get; set; }
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
    }
}