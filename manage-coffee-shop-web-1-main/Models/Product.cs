using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } // Tên sản phẩm

        [StringLength(500)]
        public string Description { get; set; } // Mô tả

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } // Giá sản phẩm

        [StringLength(200)]
        public string Image { get; set; } // Hình ảnh

        public int ViewCount { get; set; } = 0; // Số lượt xem chi tiết
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo

        [Required]
        public int CategoryId { get; set; } // Khóa ngoại đến Category
        public Category Category { get; set; } // Navigation property

        public ICollection<OrderDetail> OrderDetails { get; set; } // Quan hệ 1-n với OrderDetail

    }
}
