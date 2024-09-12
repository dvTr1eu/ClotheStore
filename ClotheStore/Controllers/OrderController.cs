using AspNetCoreHero.ToastNotification.Abstractions;
using ClotheStore.Models;
using ClotheStore.ModelView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClotheStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }

        public OrderController(ClothesStoreContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanId))
                {
                    return RedirectToAction("Login", "Account");
                }
                var khachHang = _context.Customers
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanId));
                if (khachHang == null)
                {
                    return NotFound();
                }
                var donHang = await _context.Orders
                    .Include(x => x.TransactStatus)
                    .Include(x => x.OrderDetails)
                    .FirstOrDefaultAsync(x => x.OrderId == id && Convert.ToInt32(taikhoanId) == x.CustomerId);
                if (donHang == null)
                {
                    return NotFound();
                }
                var chitietDh = _context.OrderDetails
                    .Include(x => x.Product)
                    .AsNoTracking()
                    .Where(x => x.OrderId == id)
                    .OrderBy(x => x.OrderDetailId)
                    .ToList();

                XemDonHang donhang = new XemDonHang();
                donhang.DonHang = donHang;
                donhang.ChiTietDh = chitietDh;

                return PartialView("Details", donhang);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public JsonResult UpdateOrderStatus(int orderId)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.TransactStatusId = 4;
                    order.IsHistory = true;
                    order.ShipDate = DateTime.Now;
                    order.Paid = true;
                    order.PaymentDate = DateTime.Now;
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }
    }
}
