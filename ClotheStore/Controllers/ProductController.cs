using ClotheStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;

namespace ClotheStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ClothesStoreContext _context;

        public ProductController(ClothesStoreContext context)
        {
            _context = context;
        }

        [Route("ao.html")]
        public IActionResult IndexShirt(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 12;
                int parentCatId = 1;
                var lsShirt = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)
                    .ThenInclude(x => x.Size)
                    .Join(_context.Categories,
                        p => p.CatId,
                        c => c.CatId,
                        (p, c) => new { Product = p, Category = c })
                    .Join(_context.Categories,
                        pc => pc.Category.ParentId,
                        parent_c => parent_c.CatId,
                        (pc, parent_c) => new { pc.Product, ParentCategory = parent_c })
                    .Where(pc => pc.ParentCategory.CatId == parentCatId)
                    .Select(pc => pc.Product);

                PagedList<Product> models = new PagedList<Product>(lsShirt.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("quan.html")]

        public IActionResult IndexPant(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 12;
                int parentCatId = 2;
                var lsShirt = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)
                    .ThenInclude(x => x.Size)
                    .Join(_context.Categories,
                        p => p.CatId,
                        c => c.CatId,
                        (p, c) => new { Product = p, Category = c })
                    .Join(_context.Categories,
                        pc => pc.Category.ParentId,
                        parent_c => parent_c.CatId,
                        (pc, parent_c) => new { pc.Product, ParentCategory = parent_c })
                    .Where(pc => pc.ParentCategory.CatId == parentCatId)
                    .Select(pc => pc.Product);

                PagedList<Product> models = new PagedList<Product>(lsShirt.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("ao-khoac.html")]

        public IActionResult IndexOutwear(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 12;
                int parentCatId = 3;
                var lsShirt = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)
                    .ThenInclude(x => x.Size)
                    .Join(_context.Categories,
                        p => p.CatId,
                        c => c.CatId,
                        (p, c) => new { Product = p, Category = c })
                    .Join(_context.Categories,
                        pc => pc.Category.ParentId,
                        parent_c => parent_c.CatId,
                        (pc, parent_c) => new { pc.Product, ParentCategory = parent_c })
                    .Where(pc => pc.ParentCategory.CatId == parentCatId)
                    .Select(pc => pc.Product);

                PagedList<Product> models = new PagedList<Product>(lsShirt.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult ListSort(string sortProducts, int parentId, int level)
        {
            try
            {
                ViewBag.CurrentSortProducts = sortProducts;
                ViewBag.ParentId = parentId;
                ViewBag.Level = level;

                List<Product> productsQuery = new List<Product>(); 
                productsQuery = _context.Products
                    .Include(x => x.Cat)
                    .Include(s => s.ProductImages)
                    .Include(s => s.ProductSizes)
                    .ThenInclude(s => s.Size)
                    .Where(p => p.Cat.ParentId == parentId && p.Cat.Levels == level).ToList();
                switch (sortProducts)
                {
                    case "lowToHigh":
                        productsQuery = productsQuery.OrderBy(s => s.Price).ToList();
                        break;
                    case "highToLow":
                        productsQuery = productsQuery.OrderByDescending(s => s.Price).ToList();
                        break;
                    default:
                        productsQuery = productsQuery.OrderBy(s => s.ProductId).ToList();
                        break;
                }
                return PartialView("ListSort", productsQuery);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("malefashion/{id}/{parentId}/{level}/{ordering}")]
        public IActionResult Details(int id, int parentId, int level, int ordering)
        {
            try
            {
                var product = _context.Products
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)!
                    .ThenInclude(x => x.Size)
                    .FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<Product> lsRealatedProducts = new List<Product>();
                lsRealatedProducts = _context.Products
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)
                    .Include(x => x.Cat)
                    .Where(p => p.Cat.ParentId == parentId
                                && p.Cat.Levels == level
                                && p.Cat.Ordering == ordering
                                && p.ProductId != id)
                    .ToList();
                ViewBag.sanPhamTuongTu = lsRealatedProducts;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
