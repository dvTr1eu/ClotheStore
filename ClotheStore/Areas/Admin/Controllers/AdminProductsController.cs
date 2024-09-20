using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClotheStore.Models;
using PagedList.Core;
using ClotheStore.Helper;
using System.Collections.Immutable;
using Microsoft.Data.SqlClient;
using ClotheStore.ModelView;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;

namespace ClotheStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly ClothesStoreContext _context;
        public INotyfService _notyfService { get; }

        public AdminProductsController(ClothesStoreContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [ResponseCache(Duration = 60)]
        public IActionResult Index(int page = 1, int CatId = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Product> lsProducts = new List<Product>();

            if (CatId != 0)
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == CatId)
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)!
                .ThenInclude(ps => ps.Size)
                .OrderByDescending(x => x.CreateDate).ToList();
            }
            else
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductSizes)!
                .ThenInclude(ps => ps.Size)
                .OrderByDescending(x => x.CreateDate).ToList();
            }
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatId);
            ViewBag.CurrentCatId = CatId;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public IActionResult Fillter(int CatId = 0)
        {
            var url = $"/Admin/AdminProducts?CatId={CatId}";
            if (CatId == 0)
            {
                url = $"/Admin/AdminProducts";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)!
                .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["Sizes"] = _context.Sizes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,Price,Discount,CreateDate,CatId,BestSeller,HomeFlag,Active,Title,Alias,UnitslnStock,Cat,Size")] Product product, List<int> selectedSizes, List<int> quantities, List<Microsoft.AspNetCore.Http.IFormFile> fImages)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Utilities.ToTitleCase(product.ProductName);
                product.Alias = Utilities.SEOUrl(product.ProductName);
                if(product.CreateDate == null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _context.Add(product);

                for (int i = 0; i < selectedSizes.Count; i++)
                {
                    var productSize = new ProductSize
                    {
                        ProductId = product.ProductId,
                        SizeId = selectedSizes[i],
                        Quantity = quantities[i]
                    };
                    _context.ProductSizes.Add(productSize);
                }
               
                if (fImages != null && fImages.Count > 0) 
                {
                    for (int i = 0; i < fImages.Count && i < 2; i++)
                    {
                        var fImage = fImages[i];
                        string extension = Path.GetExtension(fImage.FileName);
                        string imageName = Utilities.SEOUrl(product.ProductName) + (i + 1).ToString() + extension;
                        string imagePath = await Utilities.UploadFile(fImage, @"sanphamImg", imageName.ToLower());

                        var productImage = new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageUrl = imagePath
                        };
                        _context.ProductImages.Add(productImage);
                    }
                }
                _notyfService.Success("Thêm thành công sản phẩm mới");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["Sizes"] = _context.Sizes.ToList();
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p .Cat)
                                        .Include(p => p.ProductImages)
                                        .Include(p => p.ProductSizes)!
                                        .ThenInclude(ps => ps.Size)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["Sizes"] = _context.Sizes.ToList();
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,CatId,Description,Price,Discount,CreateDate,BestSeller,HomeFlag,Active,Title,Alias,UnitslnStock")] Product product, List<int> selectedSizes, List<int> quantities, List<Microsoft.AspNetCore.Http.IFormFile> fImages)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products
                                                    .Include(p => p.Cat)
                                                    .Include(p => p.ProductImages)
                                                    .Include(p => p.ProductSizes)!
                                                    .ThenInclude(ps => ps.Size)
                                                    .FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    product.Alias = Utilities.SEOUrl(product.ProductName);
                    existingProduct.CatId = product.CatId;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Discount = product.Discount;
                    existingProduct.CreateDate = product.CreateDate;
                    existingProduct.BestSeller = product.BestSeller;
                    existingProduct.HomeFlag = product.HomeFlag;
                    existingProduct.Active = product.Active;
                    existingProduct.Title = product.Title;
                    _context.Update(existingProduct);

                    // Cập nhật kích thước
                    var sizesToRemove = existingProduct.ProductSizes
                                        .Where(ps => !selectedSizes.Contains(ps.SizeId))
                                        .ToList();
                    _context.ProductSizes.RemoveRange(sizesToRemove);
                    for (int i = 0; i < selectedSizes.Count; i++)
                    {
                        var sizeId = selectedSizes[i];
                        var quantity = quantities[i];

                        var existingSize = existingProduct.ProductSizes
                            .FirstOrDefault(ps => ps.SizeId == sizeId);

                        if (existingSize != null)
                        {
                            existingSize.Quantity = quantity;
                        }
                        else
                        {
                            var productSize = new ProductSize
                            {
                                ProductId = product.ProductId,
                                SizeId = sizeId,
                                Quantity = quantity,
                            };
                            _context.ProductSizes.Add(productSize);
                        }
                    }

                    if (fImages != null && fImages.Count > 0)
                    {
                        var imagesToRemove = existingProduct.ProductImages
                                            .Where(pi => !fImages.Select(f => Utilities.SEOUrl(product.ProductName) + Path.GetExtension(f.FileName)).Contains(pi.ImageUrl))
                                            .ToList();
                        _context.ProductImages.RemoveRange(imagesToRemove);
                        await _context.SaveChangesAsync();

                        for (int i = 0; i < fImages.Count && i < 2; i++)
                        {
                            var fImage = fImages[i];
                            string extension = Path.GetExtension(fImage.FileName);
                            string imageName = Utilities.SEOUrl(product.ProductName) + (i + 1).ToString() + extension;
                            string imagePath = await Utilities.UploadFile(fImage, @"sanphamImg", imageName.ToLower());

                            var existingImage = existingProduct.ProductImages.FirstOrDefault(pi => pi.ImageUrl == imageName);
                            if (existingImage != null)
                            {
                                existingImage.ImageUrl = imagePath;
                            }
                            else
                            {
                                var productImage = new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImageUrl = imagePath
                                };
                                _context.ProductImages.Add(productImage);
                            }
                        }
                    }
                    _context.Update(existingProduct);
                    _notyfService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewData["Sizes"] = _context.Sizes.ToList();
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _notyfService.Success("Xóa thành công");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
