using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } // Khóa ngoại đến ApplicationUser

        public ApplicationUser ApplicationUser { get; set; } // Navigation property

        public ICollection<CartItem> CartItems { get; set; } // Danh sách các item trong giỏ
    }
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; } // Khóa ngoại đến Cart

        [Required]
        public int ProductId { get; set; } // Khóa ngoại đến Product

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } // Số lượng

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } // Giá tại thời điểm thêm vào giỏ 

        [StringLength(200)]
        public string Notes { get; set; } // Ghi chú 

        public Cart Cart { get; set; } // Navigation property
        public Product Product { get; set; } // Navigation property
    }
}
