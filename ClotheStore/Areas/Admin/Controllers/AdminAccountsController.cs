using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClotheStore.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ClotheStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }

        public AdminAccountsController(ClothesStoreContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {

            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "1" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "0" });
            ViewData["TrangThai"] = lsTrangThai;

            var clothesStoreContext = _context.Accounts.Include(a => a.Role);
            return View(await clothesStoreContext.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
