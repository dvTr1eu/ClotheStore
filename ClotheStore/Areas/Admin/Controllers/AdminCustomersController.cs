using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClotheStore.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;

namespace ClotheStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCustomersController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }

        public AdminCustomersController(ClothesStoreContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminCustomers
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsCustomer = _context.Customers
                            .AsNoTracking()
                            .Include(x => x.Location)
                            .OrderByDescending(x => x.CreateDate);
            PagedList<Customer> models = new PagedList<Customer>(lsCustomer, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                _notyfService.Success("Xóa thành công");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
