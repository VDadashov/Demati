using Demati.DataAccessLayer;
using Demati.Interfaces;
using Demati.Models;
using Demati.ViewModels.AccountVMs;
using Demati.ViewModels.BasketVMs;
using Demati.ViewModels.ProductVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Demati.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;


        public LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IEnumerable<BasketVM>> GetBasket()
        {
            string? cookie = _contextAccessor.HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product? product = await _context.Products
                        .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                        .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                        .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                    if (product != null && product.ProductColors != null && product.ProductSizes != null)
                    {
                        basketVM.Image = product.MainImage;
                        basketVM.Title = product.Title;
                        basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                        basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;
                        basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                    }
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            return basketVMs;
        }
        public async Task<IEnumerable<WishlistVM>> GetWishlist()
        {
            string? cookie = _contextAccessor.HttpContext.Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
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
            else
            {
                wishlistVMs = new List<WishlistVM>();
            }

            return wishlistVMs;
        }

        public async Task<IEnumerable<Brand>> GetBrands()
        {
            IEnumerable<Brand> brands = await _context.Brands
                .Where(b => b.IsDeleted == false)
                .ToListAsync();

            return brands;
        }

        public async Task<IEnumerable<Setting>> GetSettings()
        {
            IEnumerable<Setting> settings = await _context.Settings.ToListAsync();

            return settings;
        }
    }
}
