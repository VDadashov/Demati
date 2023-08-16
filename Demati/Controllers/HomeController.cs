using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.HomeVMs;
using Demati.ViewModels.ProductVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Demati.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = await _context.Sliders
                .Where(s => s.IsDeleted == false)
                .ToListAsync(),
                Featured = await _context.Products
                .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false))
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors.Where(pc =>pc.IsDeleted == false))
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .Where(p => p.IsDeleted == false && p.IsFeatured)
                .ToListAsync(),
                NewArrivals = await _context.Products
                .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false))
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false))
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .Where(p => p.IsDeleted == false && p.IsNewArrivals)
                .ToListAsync(),
                Bestseller = await _context.Products
                .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false))
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false))
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .Where(p => p.IsDeleted == false && p.IsBestseller)
                .ToListAsync(),
                Banners = await _context.Banners
                .Where(b => b.IsDeleted == false)
                .ToListAsync(),
                Blogs = await _context.Blogs
                .Where(bl => bl.IsDeleted == false)
                .ToListAsync()
            };

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

            return View(homeVM);
        }
    }
}
