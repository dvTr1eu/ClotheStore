using AspNetCoreHero.ToastNotification.Abstractions;
using ClotheStore.Extension;
using ClotheStore.Models;
using ClotheStore.ModelView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ClotheStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(ClothesStoreContext context, INotyfService notyfService, ILogger<ShoppingCartController> logger)
        {
            _context = context;
            _notyfService = notyfService;
            _logger = logger;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gioHang = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gioHang == default(List<CartItem>))
                {
                    gioHang = new List<CartItem>();
                }
                return gioHang;

            }
        }
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productId, int? amount, string size, string urlImg)
        {
            List<CartItem> gioHang = GioHang;

            try
            {
                CartItem item = GioHang.
                    SingleOrDefault(
                        p => p.product.ProductId == productId
                        && p.size == size
                        && p.urlImg == urlImg);
                if (item == null)
                {
                    Product hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                    ProductSize sizeFull = _context.ProductSizes.FirstOrDefault(ps => ps.Size.SizeName == size);

                    if (hangHoa != null)
                    {
                        item = new CartItem
                        {
                            amount = amount.HasValue ? amount.Value : 1,
                            product = hangHoa,
                            productSize = sizeFull,
                            size = size,
                            urlImg = urlImg
                        };
                        gioHang.Add(item);
                    }
                }
                else
                {
                    for (int i = 0; i < gioHang.Count; i++)
                    {
                        if (gioHang[i].size == size && gioHang[i].urlImg == urlImg)
                        {
                            gioHang[i].amount = item.amount + amount.Value;
                        }
                    }
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notyfService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateAllCart([FromBody] List<CartItemDTO> cartItems)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");

            try
            {
                if (cart != null && cartItems != null)
                {
                    foreach (var cartItem in cartItems)
                    {
                        CartItem item = cart.FirstOrDefault(p => p.product.ProductId == cartItem.ProductId);

                        if (item != null && cartItem.Amount.HasValue)
                        {
                            item.amount = cartItem.Amount.Value;
                        }
                    }

                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult RemoveCart(int productId, string size, string urlImg)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang
                    .SingleOrDefault(p => p.product.ProductId == productId
                                          && p.size == size
                                          && p.urlImg == urlImg);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }

        [Route("giohang.html", Name = "Giohang")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
