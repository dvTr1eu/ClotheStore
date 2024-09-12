using ClotheStore.Models;

namespace ClotheStore.ModelView
{
    public class XemDonHang
    {
        public Order DonHang { get; set; }
        public List<OrderDetail> ChiTietDh { get; set; }
    }
}
