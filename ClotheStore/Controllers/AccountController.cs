using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using ClotheStore.Helper;
using ClotheStore.Models;
using ClotheStore.ModelView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClotheStore.Extension;
using NuGet.Packaging.Signing;
using static ClotheStore.Helper.SendMailHelper;

namespace ClotheStore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }
        private readonly SendMailHelper _sendMailHelper;

        public AccountController(ClothesStoreContext context, INotyfService notyfService, SendMailHelper sendMailHelper)
        {
            _context = context;
            _notyfService = notyfService;
            _sendMailHelper = sendMailHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachHang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachHang != null)
                {
                    return Json(data: "Số điện thoại: " + Phone + " đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachHang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Email.ToLower());
                if (khachHang != null)
                {
                    return Json(data: "Email: " + Email + " đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
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

        [HttpGet]
        [Route("my-account.html")]
        public IActionResult Index()
        {
            var taikhoanId = HttpContext.Session.GetString("CustomerId");
            if (taikhoanId != null)
            {
                var khachHang = _context.Customers
                    .AsNoTracking()
                    .Include(x => x.Location)
                    .FirstOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanId));
                if (khachHang != null)
                {
                    var lsDonHang = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == khachHang.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DanhSachDH = lsDonHang;
                    string phuongxa = khachHang.Ward.HasValue ? GetNameLocation(khachHang.Ward.Value) : string.Empty;
                    string quanhuyen = khachHang.District.HasValue ? GetNameLocation(khachHang.District.Value) : string.Empty;
                    string tinhthanh = khachHang.LocationId.HasValue ? GetNameLocation(khachHang.LocationId.Value) : string.Empty;
                    string fullAddress = $"{khachHang.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
                    ViewBag.FullAddress = fullAddress ;
                    return View(khachHang);
                }
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(Customer model, IFormFile Avatar)
        {
            var khachHang = await _context.Customers
                .FindAsync(model.CustomerId);
            if (khachHang != null)
            {
                if (Avatar != null && Avatar.Length > 0)
                {
                    if (Avatar.Length > 819200)
                    {
                        ModelState.AddModelError("fAvatar", "Kích thước ảnh quá lớn.");
                        return Redirect("/my-account.html");
                    }

                    string fileExtension = Path.GetExtension(Avatar.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".gif" && fileExtension != ".png")
                    {
                        ModelState.AddModelError("Avatar", "Chỉ chấp nhận file JPG, GIF, hoặc PNG");
                        return Redirect("/my-account.html");
                    }

                    string fileName = Path.GetFileName(Avatar.FileName);
                    string filePath = await Utilities.UploadFile(Avatar, @"avatarImg", fileName.ToLower());

                    if (!string.IsNullOrEmpty(khachHang.Avatar))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatarImg", khachHang.Avatar);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    khachHang.Avatar = filePath;
                }
                _context.Update(khachHang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Cập nhật ảnh đại diện thành công");
                return Redirect("/my-account.html");
            }
            return Redirect("/my-account.html");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterViewModel taikhoan)
        {
            try
            {
                bool isEmail = Utilities.IsValidateEmail(taikhoan.Email);
                if (isEmail)
                {
                    if (ModelState.IsValid)
                    {
                        string salt = Utilities.RandomKey();
                        Customer khachHang = new Customer
                        {
                            FullName = taikhoan.CustomerName,
                            Phone = taikhoan.Phone.Trim().ToLower(),
                            Email = taikhoan.Email.Trim().ToLower(),
                            Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                            Salt = salt,
                            Active = true,
                            CreateDate = DateTime.Now,
                            IsEmailConfirmed = false
                        };
                        bool existEmail = _context.Customers.Any(x => x.Email == taikhoan.Email);
                        try
                        {
                            if (!existEmail)
                            {
                                var confirmationLink = Url.Action("ConfirmEmail", "Account", new
                                {
                                    token = khachHang.CustomerId,
                                    email = khachHang.Email
                                }, Request.Scheme);

                                var mailContent = new MailContent
                                {
                                    ToEmail = khachHang.Email,
                                    Subject = "XÁC THỰC TÀI KHOẢN",
                                    Body = $"<p>Vui lòng xác thực tài khoản bằng cách nhấp vào liên kết sau: <a href='{confirmationLink}'>Xác thực tài khoản</a></p>"
                                };

                                var sendMail = await _sendMailHelper.SendMail(mailContent);

                                if (sendMail.Equals("Thành công"))
                                {
                                    _context.Add(khachHang);
                                    await _context.SaveChangesAsync();

                                    HttpContext.Session.SetString("CustomerId", khachHang.CustomerId.ToString());
                                    var taiKhoanId = HttpContext.Session.GetString("CustomerId");

                                    var claims = new List<Claim>
                                    {
                                    new Claim(ClaimTypes.Name, khachHang.FullName),
                                    new Claim("CustomerId", khachHang.CustomerId.ToString())
                                    };
                                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                    await HttpContext.SignInAsync(claimsPrincipal);
                                    //return Redirect("/dang-nhap.html");
                                    return RedirectToAction("Login", "Account");
                                }
                            }
                            return View(taikhoan);
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("DangKyTaiKhoan", "Account");
                        }
                    }
                    else
                    {
                        return View(taikhoan);
                    }
                }
                 ModelState.AddModelError(string.Empty, "Email không hợp lệ");
                 return View(taikhoan);
            }
            catch
            {
                return View(taikhoan);
            }
        }

        public async Task<IActionResult> ConfirmEmail(int token, string email)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == token && c.Email == email);

            if (customer != null)
            {
                customer.IsEmailConfirmed = true;
                _context.Update(customer);
                await _context.SaveChangesAsync();

                _notyfService.Success("Xác thực email thành công, bạn có thể đăng nhập.");
                return View("ConfirmEmailSuccess");
            }

            _notyfService.Error("Xác thực email không thành công.");
            return View("ConfirmEmailFailure");
        }

        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string? returnUrl)
        {
            var taikhoanId = HttpContext.Session.GetString("CustomerId");
            if (taikhoanId != null)
            {
                return RedirectToAction("Index", "Account");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string? returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidateEmail(customer.Username);
                    if (!isEmail)
                    {
                        ModelState.AddModelError(string.Empty, "Email không hợp lệ");
                        return View(customer);
                    }

                    var khachHang = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == customer.Username);

                    if (khachHang == null)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại");
                        return RedirectToAction("DangKyTaiKhoan", "Account");
                    }
                    else if(khachHang != null && khachHang.IsEmailConfirmed == false)
                    {
                        ModelState.AddModelError(string.Empty, "Chưa xác thực email");
                        return View(customer);
                    }

                    string pass = (customer.Password + khachHang.Salt.Trim()).ToMD5();
                    if (khachHang.Password != pass)
                    {
                        ModelState.AddModelError(string.Empty, "Mật khẩu không chính xác");
                        return View(customer);
                    }

                    HttpContext.Session.SetString("CustomerId", khachHang.CustomerId.ToString());
                    var taiKhoanId = HttpContext.Session.GetString("CustomerId");

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachHang.FullName),
                            new Claim("CustomerId", khachHang.CustomerId.ToString())
                        };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    if (!string.IsNullOrEmpty(Request.Form["returnUrl"]))
                    {
                        returnUrl = Request.Form["returnUrl"];
                    }

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Account");
                }
            }
            catch (Exception ex)
            {
                _notyfService.Error("Đã xảy ra lỗi khi đăng nhập");
                return RedirectToAction("Login", "Account");
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanId = HttpContext.Session.GetString("CustomerId");
                if (taikhoanId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanId));
                    if (taikhoan == null) return RedirectToAction("Index", "Home");

                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    if (pass == taikhoan.Password)
                    {
                        string passNew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passNew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Cập nhật thành công");
                        return Redirect("/my-account.html");
                    }
                }
            }
            catch
            {
                _notyfService.Error("Cập nhật không thành công");
                return Redirect("/my-account.html");
            }
            _notyfService.Error("Cập nhật không thành công");
            return Redirect("/my-account.html");
        }
    }
}
