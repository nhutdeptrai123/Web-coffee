using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace manage_coffee_shop_web.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(50)]
        public string Code { get; set; } // Mã coupon 

        [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; } // Phần trăm giảm giá

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm giá phải là số >= 0")]
        public decimal DiscountAmount { get; set; } // Số tiền giảm giá cố định

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Display(Name = "Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } // Ngày bắt đầu

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Display(Name = "Ngày kết thúc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } // Ngày kết thúc

        public bool IsActive { get; set; } = true; // Trạng thái coupon
    }
}
