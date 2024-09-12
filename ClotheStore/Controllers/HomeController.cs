using ClotheStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ClotheStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClothesStoreContext _context;

        public HomeController(ClothesStoreContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var lsBestSale = _context.Products
                .Include(s => s.Cat)
                .Include(s => s.ProductImages)
                .Include(s => s.ProductSizes)
                .ThenInclude(s => s.Size)
                .Where(s => s.BestSeller == true)
                .ToList();
            ViewBag.BestSale = lsBestSale;

            var newArrival = _context.Products
                .Include(s => s.Cat)
                .Include(s => s.ProductImages)
                .Include(s => s.ProductSizes)
                .ThenInclude(s => s.Size)
                .Where(s => s.HomeFlag == true)
                .ToList();
            ViewBag.NewArrival = newArrival;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
