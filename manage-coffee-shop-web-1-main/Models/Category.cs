using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Tên danh mục

        public ICollection<Product> Products { get; set; } // Quan hệ 1-n với Product
    }
}
