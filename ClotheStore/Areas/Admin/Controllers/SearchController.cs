using AspNetCoreHero.ToastNotification.Abstractions;
using ClotheStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClotheStore.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class SearchController : Controller
    {
        private readonly ClothesStoreContext _context;
        public SearchController(ClothesStoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string key)
        {
            List<Product> ls = new List<Product>();
            
            if (string.IsNullOrEmpty(key) || key.Length < 1)
            {
                ls = _context.Products
               .AsNoTracking()
               .Include(p => p.Cat)
               .Include(p => p.ProductImages)
               .Include(p => p.ProductSizes)!
                   .ThenInclude(p => p.Size)
               .Where(p => p.ProductName.Contains(key))
               .OrderByDescending(p => p.CreateDate)
               .Take(10)
               .ToList();
            }
            else
            {
                ls = _context.Products
               .AsNoTracking()
               .Include(p => p.Cat)
               .Include(p => p.ProductImages)
               .Include(p => p.ProductSizes)!
                   .ThenInclude(p => p.Size)
               .Where(p => p.ProductName.Contains(key))
               .OrderByDescending(p => p.CreateDate)
               .Take(10)
               .ToList();
            }
            return PartialView("ListProductSearchPartial", ls);
        }

        public IActionResult FindOrder(string key)
        {
            List<Order> ls = new List<Order>();

            if (string.IsNullOrWhiteSpace(key) || key.Equals(""))
            {
                ls = _context.Orders
                   .AsNoTracking()
                   .Include(p => p.Customer)
                   .Include(p => p.OrderDetails)
                   .Include(p => p.TransactStatus)
                   .Where(p => p.Customer.FullName.Contains(key))
                   .ToList();
            }
            else
            {
                ls = _context.Orders
                   .AsNoTracking()
                   .Include(p => p.Customer)
                   .Include(p => p.OrderDetails)
                   .Include(p => p.TransactStatus)
                   .Where(p => p.Customer.FullName.Contains(key))
                   .OrderByDescending(p => p.OrderDate)
                   .Take(10)
                   .ToList();
            }
            return PartialView("ListOrderSearchPartial", ls);
        }
    }
}
