using System.Diagnostics;
using manage_coffee_shop_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace manage_coffee_shop_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("MyDb");
            _logger.LogInformation("Loaded connection string: {ConnectionString}", connectionString ?? "null");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string 'MyDb' is not configured.");
                return View();
            }

            var banner = GetBannerFromDatabase();
            if (banner != null)
            {
                ViewBag.BannerImagePath = banner.Image;
                ViewBag.BannerTitle = banner.Title;
                ViewBag.BannerDescription = banner.Description;
                _logger.LogInformation("Banner retrieved: Image={Image}, Title={Title}, Description={Description}", banner.Image, banner.Title, banner.Description);
            }
            else
            {
                ViewBag.BannerImagePath = null;
                ViewBag.BannerTitle = "";
                ViewBag.BannerDescription = "";
                _logger.LogWarning("No banner data retrieved from database.");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Banner GetBannerFromDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("MyDb");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Connection string is null or empty in GetBannerFromDatabase.");
                    return null;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT TOP 1 Id, Title, Description, Image FROM Banner ORDER BY Id DESC";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Banner
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Image = reader["Image"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving banner from database");
            }
            return null;
        }
    }
}