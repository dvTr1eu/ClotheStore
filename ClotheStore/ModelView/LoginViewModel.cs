using System.ComponentModel.DataAnnotations;

namespace ClotheStore.ModelView
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu ít nhất 8 ký tự")]
        public string Password { get; set; }    
    }
}
