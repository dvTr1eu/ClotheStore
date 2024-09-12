using AspNetCoreHero.ToastNotification.Abstractions;
using ClotheStore.Extension;
using ClotheStore.Models;
using ClotheStore.ModelView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace ClotheStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }
        public AdminOrdersController(ClothesStoreContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsOrders = _context.Orders
                .Include(d => d.Customer)
                .Include(d => d.TransactStatus)
                .AsNoTracking()
                .OrderByDescending(d => d.OrderDate);
            PagedList<Order> models = new PagedList<Order>(lsOrders, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public string GetNameLocation(int idLocation)
        {
            try
            {
                var location = _context.Locations
                    .AsNoTracking()
                    .SingleOrDefault(x => x.LocationId == idLocation);
                if (location != null)
                {
                    return location.NameWithType;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var donhang = await _context.Orders
                .Include(d => d.Customer)
                .Include(d => d.TransactStatus)
                .Include(d => d.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (donhang == null)
            {
                return NotFound();
            }

            //var khachHangAddress = (from c in _context.Customers
            //                        join d in _context.Locations on c.District equals d.LocationId
            //                        join w in _context.Locations on c.Ward equals w.LocationId
            //                        where c.CustomerId == Convert.ToInt32(donhang.CustomerId)
            //                        select new
            //                        {
            //                            Address = c.Address,
            //                            DistrictName = d.Name,
            //                            WardName = w.Name,
            //                            CityName = (from p in _context.Locations where p.LocationId == c.LocationId select p.NameWithType).FirstOrDefault()
            //                        }).FirstOrDefault();
            string phuongxa = donhang.Customer.Ward.HasValue ? GetNameLocation(donhang.Customer.Ward.Value) : string.Empty;
            string quanhuyen = donhang.Customer.District.HasValue ? GetNameLocation(donhang.Customer.District.Value) : string.Empty;
            string tinhthanh = donhang.Customer.LocationId.HasValue ? GetNameLocation(donhang.Customer.LocationId.Value) : string.Empty;

            ViewBag.Chitiet = donhang.OrderDetails;
            string fullAddress = $"{donhang.Customer.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
            ViewBag.FullAddress = fullAddress;
            return View(donhang);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(d => d.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(x => x.OrderId == Convert.ToInt32(id));
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);

            string phuongxa = order.Customer.Ward.HasValue ? GetNameLocation(order.Customer.Ward.Value) : string.Empty;
            string quanhuyen = order.Customer.District.HasValue ? GetNameLocation(order.Customer.District.Value) : string.Empty;
            string tinhthanh = order.Customer.LocationId.HasValue ? GetNameLocation(order.Customer.LocationId.Value) : string.Empty;
            string fullAddress = $"{order.Customer.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
            ViewBag.FullAddress = fullAddress;
            var orderVm = new DonHangVM()
            {
                IdDh = order.OrderId,
                CustomId = order.CustomerId,
                Ghichu = order.Note,
                Idthanhtoan = order.PaymentId,
                IdTrangthai = order.TransactStatusId,
                Ngaygiao = order.ShipDate,
                Ngaytao = order.OrderDate,
                Ngaythanhtoan = order.PaymentDate,
                Thanhtoan = (bool)order.Paid,
                Tongtien = order.TotalMoney,
                Xoadonhang = (bool)order.Deleted,
                Customer = order.Customer
            };
            return PartialView("ChangeStatus", orderVm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, DonHangVM model)
        {
            if (id != model.IdDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Orders
                        .AsNoTracking()
                        .Include(x => x.Customer)
                        .FirstOrDefaultAsync(x => x.OrderId == Convert.ToInt32(id));
                    if (donhang != null)
                    {
                        donhang.Paid = model.Thanhtoan;
                        donhang.Deleted = model.Xoadonhang;
                        donhang.TransactStatusId = model.IdTrangthai;
                        if (donhang.Paid == true)
                        {
                            donhang.PaymentDate = DateTime.Now;
                        }
                        if (donhang.TransactStatusId == 5)
                        {
                            donhang.Deleted = true;
                        }
                        if (donhang.TransactStatusId == 3)
                        {
                            //DateTime ngayTao = Convert.ToDateTime(donhang.OrderDate);
                            //donhang.ShipDate = ngayTao.AddDays(3);
                            donhang.ShipDate = DateTime.Now;
                        }
                    }
                    _context.Update(donhang);
                    _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái đơn hàng thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonhangExists(model.IdDh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "IdTrangthai", "Trangthai", model.IdTrangthai);
            return PartialView("ChangeStatus", model);
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var donhang = await _context.Orders.FindAsync(id);
            if (donhang == null)
            {
                return NotFound();
            }
            else
            {
                donhang.Deleted = true;
                _context.Orders.Update(donhang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Hủy đơn hàng thành công");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var donhang = await _context.Orders.FindAsync(id);
            if (donhang == null)
            {
                return NotFound();
            }
            else
            {
                _context.Remove(donhang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Xóa đơn hàng thành công");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool DonhangExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
