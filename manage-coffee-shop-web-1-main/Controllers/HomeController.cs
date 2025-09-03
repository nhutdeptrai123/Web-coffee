using System.Diagnostics;
using manage_coffee_shop_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace manage_coffee_shop_web.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IActionResult Index(int? categoryId = null, int? menuCategoryId = null)
        {
            var connectionString = _configuration.GetConnectionString("MyDb");
            _logger.LogInformation("Loaded connection string: {ConnectionString} at {Time}", connectionString ?? "null", DateTime.Now);

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string 'MyDb' is not configured at {Time}", DateTime.Now);
                return View();
            }

            var banner = GetBannerFromDatabase();
            var categories = GetCategoriesFromDatabase();
            var products = GetProductsFromDatabase(categoryId);
            var menuItems = GetMenuItemsFromDatabase(menuCategoryId);

            _logger.LogInformation("Fetched {ProductCount} products, {CategoryCount} categories, {MenuItemCount} menu items at {Time}",
                products?.Count ?? 0, categories?.Count ?? 0, menuItems?.Count ?? 0, DateTime.Now);

            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.MenuCategories = categories; // Sử dụng chung danh mục
            ViewBag.MenuItems = menuItems;

            if (banner != null)
            {
                ViewBag.BannerImagePath = banner.Image;
                ViewBag.BannerTitle = banner.Title;
                ViewBag.BannerDescription = banner.Description;
                _logger.LogInformation("Banner retrieved: Image={Image}, Title={Title}, Description={Description} at {Time}",
                    banner.Image, banner.Title, banner.Description, DateTime.Now);
            }
            else
            {
                ViewBag.BannerImagePath = null;
                ViewBag.BannerTitle = "";
                ViewBag.BannerDescription = "";
                _logger.LogWarning("No banner data retrieved from database at {Time}", DateTime.Now);
            }

            return View();
        }

        private List<Product> GetProductsFromDatabase(int? categoryId = null)
        {
            var products = new List<Product>();
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("MyDb")))
                {
                    connection.Open();
                    var query = @"
                        SELECT Id, Name, Description, Price, Image, CategoryId, ViewCount, CreatedDate 
                        FROM Products";
                    if (categoryId.HasValue)
                    {
                        query += " WHERE CategoryId = @CategoryId";
                    }
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (categoryId.HasValue)
                        {
                            command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                            _logger.LogInformation("Filtering products with CategoryId = {CategoryId} at {Time}", categoryId.Value, DateTime.Now);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product {
                                    Id = reader.GetInt32(0),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Price = reader.GetDecimal(3),
                                    Image = reader["Image"].ToString(),
                                    CategoryId = reader.GetInt32(5),
                                    ViewCount = reader.GetInt32(6),
                                    CreatedDate = reader.GetDateTime(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from database at {Time}", DateTime.Now);
            }
            return products;
        }

        private List<Category> GetCategoriesFromDatabase()
        {
            var categories = new List<Category>();
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("MyDb")))
                {
                    connection.Open();
                    var query = "SELECT Id, Name FROM Categories";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category {
                                    Id = reader.GetInt32(0),
                                    Name = reader["Name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories from database at {Time}", DateTime.Now);
            }
            return categories;
        }

        private List<dynamic> GetMenuItemsFromDatabase(int? menuCategoryId = null)
        {
            var menuItems = new List<dynamic>();
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("MyDb")))
                {
                    connection.Open();
                    var query = @"
                        SELECT Id, Name, Price, ImageUrl, CategoryId, IsFeatured 
                        FROM MenuItems"; // Thay bằng tên bảng thực tế
                    if (menuCategoryId.HasValue)
                    {
                        query += " WHERE CategoryId = @CategoryId";
                    }
                    using (var command = new SqlCommand(query, connection))
                    {
                        if (menuCategoryId.HasValue)
                        {
                            command.Parameters.AddWithValue("@CategoryId", menuCategoryId.Value);
                            _logger.LogInformation("Filtering menu items with CategoryId = {CategoryId} at {Time}", menuCategoryId.Value, DateTime.Now);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                menuItems.Add(new {
                                    Id = reader.GetInt32(0),
                                    Name = reader["Name"].ToString(),
                                    Price = reader.GetDecimal(1),
                                    ImageUrl = reader["ImageUrl"].ToString(),
                                    CategoryId = reader.GetInt32(4),
                                    IsFeatured = reader.GetBoolean(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu items from database at {Time}", DateTime.Now);
            }
            return menuItems;
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
                    _logger.LogError("Connection string is null or empty in GetBannerFromDatabase at {Time}", DateTime.Now);
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
                                return new Banner {
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
                _logger.LogError(ex, "Error retrieving banner from database at {Time}", DateTime.Now);
            }
            return null;
        }
    }
}