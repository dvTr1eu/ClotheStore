using AspNetCoreHero.ToastNotification.Abstractions;
using Azure;
using ClotheStore.Extension;
using ClotheStore.Helper;
using ClotheStore.Models;
using ClotheStore.ModelView;
using ClotheStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace ClotheStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ClothesStoreContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly SendMailHelper _sendMailHelper;
        public INotyfService _notyfService { get; }
        public CheckoutController(ClothesStoreContext context, INotyfService notyfService, IVnPayService vnPayService, SendMailHelper sendMailHelper)
        {
            _context = context;
            _notyfService = notyfService;
            _vnPayService = vnPayService;
            _sendMailHelper = sendMailHelper;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("thanhtoan.html")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanId = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanId != null)
            {
                var khachHang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanId));
                model.CustomerId = khachHang.CustomerId;
                model.FullName = khachHang.FullName;
                model.Email = khachHang.Email;
                model.PhoneNumber = khachHang.Phone;
                model.Address = khachHang.Address;
            }

            ViewData["lsTinhThanh"] =
                new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("thanhtoan.html")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanId = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanId != null)
            {
                var khachHang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanId));
                model.CustomerId = khachHang.CustomerId;
                model.FullName = khachHang.FullName;
                model.Email = khachHang.Email;
                model.PhoneNumber = khachHang.Phone;
                model.Address = khachHang.Address;

                khachHang.LocationId = muaHang.TinhThanh;
                khachHang.District = muaHang.QuanHuyen;
                khachHang.Ward = muaHang.PhuongXa;
                khachHang.Address = muaHang.Address;

                _context.Update(khachHang);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Order donHang = new Order();
                    donHang.CustomerId = model.CustomerId;
                    donHang.OrderDate = DateTime.Now;
                    donHang.TransactStatusId = 1;
                    donHang.Deleted = false;
                    if (!string.IsNullOrEmpty(muaHang.Note))
                    {
                        donHang.Note = Utilities.StripHTML(muaHang.Note);
                    }
                    donHang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.totalMoney));

                    if (muaHang.PaymentMethod == "Cash")
                    {
                        donHang.PaymentId = 1;
                        donHang.Paid = false;
                        donHang.IsHistory = false;
                        _context.Add(donHang);
                        _context.SaveChanges();

                         CreateOrderDetails(donHang);

                        _context.SaveChanges();
                        HttpContext.Session.Remove("GioHang");
                        _notyfService.Success("Đơn đặt hàng thành công");
                        return RedirectToAction("Success");
                    }

                    if (muaHang.PaymentMethod == "Onl")
                    {
                        var vnPayModel = new VnPaymentRequestVM
                        {
                            Amount = cart.Sum(p => p.totalMoney),
                            CreatedDate = DateTime.Now,
                            Description = "Thanh toán qua VnPay",
                            FullName = muaHang.FullName,
                        };

                        HttpContext.Session.Set("TempOrder", donHang);
                        HttpContext.Session.Set("CartItems", cart);
                        return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["lsTinhThanh"] =
                    new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewData["lsTinhThanh"] =
                new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }
        public void CreateOrderDetails(Order donHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");

            foreach(var item in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = donHang.OrderId;
                orderDetail.ProductId = item.product.ProductId;
                orderDetail.Quantity = item.amount;
                orderDetail.Total = Convert.ToInt32(donHang.TotalMoney);
                orderDetail.Size = item.size;
                orderDetail.ImageUrl = item.urlImg;
                orderDetail.ShipDate = donHang.ShipDate;
                _context.Add(orderDetail);

                var productSize = _context.ProductSizes
                    .SingleOrDefault(ps => ps.ProductId == item.product.ProductId && ps.Size.SizeName == item.size);

                if (productSize != null)
                {
                    productSize.Quantity -= item.amount;
                    _context.Update(productSize);
                }
            }
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

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response != null && response.VnPayResponseCode == "00")  
            {
                // Lấy lại thông tin đơn hàng từ session
                var donHang = HttpContext.Session.Get<Order>("TempOrder");

                if (donHang != null)
                {
                    donHang.PaymentId = 2;  
                    donHang.Paid = true;
                    donHang.PaymentDate = DateTime.Now;
                    donHang.IsHistory = false;
                    _context.Add(donHang);
                    _context.SaveChanges();

                    CreateOrderDetails(donHang);

                    _context.SaveChanges();

                    HttpContext.Session.Remove("GioHang");
                    HttpContext.Session.Remove("TempOrder");

                    return RedirectToAction("Success");
                }
            }

            TempData["Message"] = $"Lỗi thanh toán VnPay: {response.VnPayResponseCode}";
            return RedirectToAction("PaymentFail");
        }

        public IActionResult PaymentFail()
        {
            ViewBag.Message = TempData["Message"] ?? "Thanh toán thất bại. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
            return View();
        }

        [Route("dat-hang-thanh-cong.html")]
        public async Task<IActionResult> Success()
        {
            try
            {
                var taikhoanId = HttpContext.Session.GetString("CustomerId");
                var khachHang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanId));
                var donHang = _context.Orders.Where(x => x.CustomerId == Convert.ToInt32(taikhoanId))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();

                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachHang.FullName;
                successVM.DonHangID = donHang.OrderId;
                successVM.Phone = khachHang.Phone;
                successVM.Address = khachHang.Address;

                string phuongxa = khachHang.Ward.HasValue ? GetNameLocation(khachHang.Ward.Value) : string.Empty;
                string quanhuyen = khachHang.District.HasValue ? GetNameLocation(khachHang.District.Value) : string.Empty;
                string tinhthanh = khachHang.LocationId.HasValue ? GetNameLocation(khachHang.LocationId.Value) : string.Empty;
                string fullAddress = $"{khachHang.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
                ViewBag.FullAddress = fullAddress;

                var mailContent = new SendMailHelper.MailContent();

                mailContent.ToEmail = $"{khachHang.Email}";
                mailContent.Subject = "XÁC NHẬN ĐẶT HÀNG THÀNH CÔNG";
                mailContent.Body = $"<h2>Xin chào, {khachHang.FullName}</h2><br/>" +
                                    $"<p>Đơn hàng #{donHang.OrderId} của bạn đã được thanh toán thành công.</p><br/>" +
                                    $"<p>Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi.</p><br/>" +
                                    $"<p>Hãy kiểm tra email thường xuyên để nhận thông tin đơn hàng và thông tin khuyến mãi nhé!</p>";
                
                var sendMail = await _sendMailHelper.SendMail(mailContent);
                ViewBag.SendMail = sendMail;
                return View(successVM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
