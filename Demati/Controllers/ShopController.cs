using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.ProductVMs;
using Demati.ViewModels.ShopVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.Design;

namespace Demati.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1)
        {
            IEnumerable<Product> products = await _context.Products
                .Include(p => p.ProductCategories.Where(pc => pc.IsDeleted == false))
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted== false))
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes.Where(pc => pc.IsDeleted == false))
                    .ThenInclude(ps => ps.Size)
                .Where(p => p.IsDeleted == false)
                .OrderBy(p => p.Title)
                .ToListAsync();


            IEnumerable<Product> reviewProducts = await _context.Products
                .Include(r => r.Reviews.Where(r => r.IsDeleted == false))
                .Where(rp => rp.IsDeleted == false)
                .ToListAsync();

            ViewBag.reviews = reviewProducts;
            ViewBag.Wishlist = null;

            string? cookie = Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            if(!string.IsNullOrEmpty(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<IEnumerable<WishlistVM>>(cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product? product = await _context.Products
                        .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                        .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishlistVM.Product = product;
                    }
                }
            }

            if (wishlistVMs != null)
            {
                ViewBag.Wishlist = wishlistVMs;
            }

            var outOfStockProducts = products.Where(p => p.Count == 0).Count();
            var inStockProducts = products.Where(p => p.Count > 0).Count();

            ViewBag.inStockCount = inStockProducts;
            ViewBag.outOfStockCount = outOfStockProducts;
            ViewBag.filterCheck = false;
            ViewBag.selectId = 4;

            if (categoryId != null)
            {
                products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId));
                ViewBag.categoryId = categoryId;
                ViewBag.filterCheck = true;
            }

            ViewBag.totalPage = (int)Math.Ceiling((decimal)products.Count() / 6);

            products = products.Skip((pageIndex - 1) * 6).Take(6);

            ViewBag.pageIndex = pageIndex;

            ViewBag.minPrice = 0;
            ViewBag.maxPrice = 50;

            ViewBag.categoryId = categoryId;

            ShopVM shopVM = new ShopVM
            {
                Products = products,
                Colors = await _context.Colors
                .Where(c => c.IsDeleted == false)
                .ToListAsync(),
                Sizes = await _context.Sizes
                .Where(s => s.IsDeleted == false)
                .ToListAsync(),
                Categories = await _context.Categories
                .Include(c => c.ProductCategories.Where(c => c.IsDeleted == false))
                .Where(c => c.IsDeleted == false)
                .OrderByDescending(p => p.ProductCategories.Count())
                .ToListAsync(),
            };

            return View(shopVM);
        }

        public async Task<IActionResult> ShopFilters(int? selectId,bool filterChecked,int? availability, int? categoryId, int? colorId, int? sizeId, double? minPrice, double? maxPrice, int pageIndex = 1)
        {
            IEnumerable<Product> products = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted ==false))
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes.Where(pc => pc.IsDeleted == false))
                    .ThenInclude(ps => ps.Size)
                .Where(p => p.IsDeleted == false)
                .OrderBy(p => p.Title)
                .ToListAsync();

            IEnumerable<Category> categories = await _context.Categories
                .Include(c => c.ProductCategories.Where(pc => pc.IsDeleted == false))
                .Where(c => c.IsDeleted == false)
                .ToListAsync();

            IEnumerable<Product> reviewProducts = await _context.Products
                .Include(r => r.Reviews.Where(r => r.IsDeleted == false))
                .Where(rp => rp.IsDeleted == false)
                .ToListAsync();

            ViewBag.reviews = reviewProducts;

            ViewBag.filterCheck = filterChecked;
            ViewBag.selectId = 4;

            ViewBag.Wishlist = null;

            string? cookie = Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            if (!string.IsNullOrEmpty(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<IEnumerable<WishlistVM>>(cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product? product = await _context.Products
                        .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                        .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishlistVM.Product = product;
                    }
                }
            }

            if (wishlistVMs != null)
            {
                ViewBag.Wishlist = wishlistVMs;
            }

            var outOfStockProducts = products.Where(p => p.Count == 0).Count();
            var inStockProducts = products.Where(p => p.Count > 0).Count();

            ViewBag.inStockCount = inStockProducts;
            ViewBag.outOfStockCount = outOfStockProducts;

            if (availability == products.Where(p => p.Count > 0).Count())
            {
                products = products.Where(p => p.Count > 0);
                ViewBag.Availability = availability;
                ViewBag.filterCheck = true;
            }

            if (availability == products.Where(p => p.Count == 0).Count())
            {
                products = products.Where(p => p.Count == 0);
                ViewBag.Availability = availability;
                ViewBag.filterCheck = true;
            }

            if (colorId != null)
            {
                products = products.Where(p => p.ProductColors.Any(pc => pc.ColorId == colorId));
                ViewBag.ColorId = colorId;
                ViewBag.filterCheck = true;
            }

            if (sizeId != null)
            {
                products = products.Where(p => p.ProductSizes.Any(ps => ps.SizeId == sizeId));
                ViewBag.SizeId = sizeId;
                ViewBag.filterCheck = true;
            }

            if (categoryId != null)
            {
                products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId));
                ViewBag.categoryId = categoryId;
                ViewBag.filterCheck = true;
            }

            if (minPrice >= 0 && maxPrice > 0)
            {
                products = products.Where(p =>(p.DisCountPrice > 0 ? 
                (p.DisCountPrice > minPrice && p.DisCountPrice < maxPrice) : (p.Price > minPrice && p.Price < maxPrice)));

                ViewBag.minPrice = minPrice;
                ViewBag.maxPrice = maxPrice;
            }

            if (selectId == 1)
            {
                products = products.Where(p => p.IsFeatured);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 2)
            {
                products = products.Where(p => p.IsBestseller);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 3)
            {
                products = products.Where(p => p.IsNewArrivals);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 4)
            {
                products = products.OrderBy(p => p.Title);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 5)
            {
                products = products.OrderByDescending(p => p.Title);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 6)
            {
                products = products.OrderBy(p => (p.DisCountPrice > 0 ? p.DisCountPrice : p.Price));
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 7)
            {
                products = products.OrderByDescending(p => (p.DisCountPrice > 0 ? p.DisCountPrice : p.Price));
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 8)
            {
                products = products.OrderBy(p => p.CreatedAt);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            if (selectId == 9)
            {
                products = products.OrderByDescending(p => p.CreatedAt);
                ViewBag.selectId = selectId;
                ViewBag.filterCheck = true;
            }

            ViewBag.totalPage = (int)Math.Ceiling((decimal)products.Count() / 6);
            products = products.Skip((pageIndex - 1) * 6).Take(6);
            ViewBag.pageIndex = pageIndex;

            ShopVM shopVM = new ShopVM
            {
                Products = products,
                Colors = await _context.Colors
                .Where(c => c.IsDeleted == false)
                .ToListAsync(),
                Sizes = await _context.Sizes
                .Where(s => s.IsDeleted == false)
                .ToListAsync(),
                Categories = categories.OrderByDescending(p => p.ProductCategories.Count())
            };

            return PartialView("_ShopListPartial", shopVM);
        }
    }
}
