using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace manage_coffee_shop_web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Tên khách hàng

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } // Số điện thoại

        [StringLength(200)]
        public string Address { get; set; } // Địa chỉ giao hàng

        public ICollection<Order> Orders { get; set; } // Quan hệ 1-n với Order

        public DateTime CreatedAt { get; set; } = DateTime.Now; // ngày tạo tài khoản
    }
}
