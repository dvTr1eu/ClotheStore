using System.ComponentModel.DataAnnotations;

namespace ClotheStore.ModelView
{
    public class MuaHangVM
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Địa chỉ nhận hàng")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public int TinhThanh { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int QuanHuyen { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public int PhuongXa { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
    }
}
