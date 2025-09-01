using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // Tiêu đề banner

        [StringLength(500)]
        public string Description { get; set; } // Mô tả ngắn về banner

        [Required]
        [StringLength(200)]
        public string Image { get; set; } // Hình ảnh banner
    }
}
