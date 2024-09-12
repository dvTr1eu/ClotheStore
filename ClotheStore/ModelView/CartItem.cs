using ClotheStore.Models;

namespace ClotheStore.ModelView
{
    public class CartItem
    {
        public Product product { get; set; }
        public ProductSize productSize { get; set; }
        public ProductImage productImage { get; set; }
        public Size sizeName { get; set; }
        public int amount { get; set; }
        public string size { get; set; }
        public string urlImg { get; set; }
        public double totalMoney => amount * product.Price.Value;

        public double Total_Money_USD()
        {
            var total = Math.Round(totalMoney / 25000, 2);
            return total;
        }
    }
}
