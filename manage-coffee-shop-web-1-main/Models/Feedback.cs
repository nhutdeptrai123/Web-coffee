using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } // Khóa ngoại đến ApplicationUser

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } // Nội dung phản hồi

        [Range(1, 5)]
        public int Rating { get; set; } // Điểm đánh giá (1-5)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo

        public ApplicationUser ApplicationUser { get; set; } // Navigation property
    }
}
