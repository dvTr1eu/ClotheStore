using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClotheStore.ModelView
{
    public class RegisterViewModel
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Account")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Account")]
        public string Phone { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu ít nhất 8 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu ít nhất 8 ký tự")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }
    }
}
