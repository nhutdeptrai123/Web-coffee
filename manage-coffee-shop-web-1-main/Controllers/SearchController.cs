using manage_coffee_shop_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace manage_coffee_shop_web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IConfiguration _configuration;

        public SearchController(ILogger<SearchController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IActionResult Index(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            var products = GetProductsFromDatabase(categoryId, searchTerm, minPrice, maxPrice);
            var categories = GetCategoriesFromDatabase();

            // Pass data to the Home view
            ViewBag.Products = products;
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Return the Home/Index view
            return View("~/Views/Home/Index.cshtml");
        }

        private List<Product> GetProductsFromDatabase(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            var products = new List<Product>();
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("MyDb")))
                {
                    connection.Open();
                    var query = "SELECT TOP 6 Id, Name, Description, Price, Image FROM Products WHERE 1=1";
                    var parameters = new List<SqlParameter>();

                    if (categoryId.HasValue)
                    {
                        query += " AND CategoryId = @CategoryId";
                        parameters.Add(new SqlParameter("@CategoryId", categoryId.Value));
                    }
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " AND Name LIKE @SearchTerm";
                        parameters.Add(new SqlParameter("@SearchTerm", $"%{searchTerm}%"));
                    }
                    if (minPrice.HasValue)
                    {
                        query += " AND Price >= @MinPrice";
                        parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
                    }
                    if (maxPrice.HasValue)
                    {
                        query += " AND Price <= @MaxPrice";
                        parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Price = reader.GetDecimal(3),
                                    Image = reader["Image"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from database");
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
                                categories.Add(new Category
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader["Name"].ToString()
                                });
                            }
                        }
                    }
                }
                if (categories.Count == 0)
                {
                    _logger.LogWarning("No categories found in the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories from database");
            }
            return categories;
        }
    }
}