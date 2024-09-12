using ClotheStore.Models;

namespace ClotheStore.ModelView
{
    public class DonHangVM
    {
        public int IdDh { get; set; }
        public DateTime? Ngaytao { get; set; }
        public double? Tongtien { get; set; }
        public int IdTrangthai { get; set; }
        public string? Ghichu { get; set; }
        public int? CustomId { get; set; }
        public DateTime? Ngaygiao { get; set; }
        public bool Thanhtoan { get; set; }
        public bool Xoadonhang { get; set; }
        public DateTime? Ngaythanhtoan { get; set; }
        public int? Idthanhtoan { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
