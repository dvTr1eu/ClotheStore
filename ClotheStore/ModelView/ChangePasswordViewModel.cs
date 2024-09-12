using System.ComponentModel.DataAnnotations;

namespace ClotheStore.ModelView
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Mật khẩu hiện tại")]
        public string PasswordNow { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu ít nhất 8 ký tự")]
        public string Password { get; set; }

        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu ít nhất 8 ký tự")]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
