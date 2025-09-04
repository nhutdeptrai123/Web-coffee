using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } // Khóa ngoại đến ApplicationUser (string thay vì int)

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Trạng thái: Pending, Completed, Cancelled

        public ApplicationUser ApplicationUser { get; set; } // Navigation property

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; } // Khóa ngoại đến Order

        [Required]
        public int ProductId { get; set; } // Khóa ngoại đến Product

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } // Số lượng

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } // Giá tại thời điểm đặt 

        [StringLength(200)]
        public string Notes { get; set; } // Thêm ghi chú
        public Order Order { get; set; } // Navigation property
        public Product Product { get; set; } // Navigation property
    }
}
