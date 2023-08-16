using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.BasketVMs;
using Demati.ViewModels.ProductVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Drawing;
using System.Net;

namespace Demati.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            ViewBag.Wishlist = null;

            string? cookie = Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            if (!string.IsNullOrEmpty(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<IEnumerable<WishlistVM>>(cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product? product1 = await _context.Products
                        .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                        .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product1 != null)
                    {
                        wishlistVM.Product = product1;
                    }
                }
            }

            if (wishlistVMs != null)
            {
                ViewBag.Wishlist = wishlistVMs;
            }

            Product product = await _context.Products
                .Include(p =>p.ProductSizes.Where(ps =>ps.ProductId == id && ps.IsDeleted == false)).ThenInclude(pc => pc.Size)
                .Include(p =>p.ProductColors.Where(pc=>pc.ProductId == id && pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId = id }
            };
            return View(productReviewVM);
        }

        public async Task<IActionResult> Search(string search)
        {

            List<Product> products = await _context.Products
                .Where(c => c.IsDeleted == false && c.Title.ToLower().Contains(search.ToLower())).ToListAsync();

            return PartialView("_SearchPartial", products);
        }

        public async Task<IActionResult> Cart() 
        {
            string? cookie = Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;

            double subTotal = 0;

            if (cookie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product? product = await _context.Products
                    .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                    .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        basketVM.Image = product.MainImage;
                        basketVM.Title = product.Title;
                        basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                        basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;
                        basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;

                        subTotal += (basketVM.Price * basketVM.Count);
                    }
                }

                ViewBag.subTotal = subTotal;

                return View(basketVMs);
            }

            return View();
        }

        public async Task<IActionResult> Modal(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductSizes.Where(ps => ps.ProductId == id && ps.IsDeleted == false)).ThenInclude(pc => pc.Size)
                .Include(p => p.ProductColors.Where(pc => pc.ProductId == id && pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            return PartialView("_ModalPartial", product);
        }

        public async Task<IActionResult> AddBasketCount(int? id, int? colorId, int? sizeId, int? count)
        {
            if (id == null) return BadRequest();

            if (colorId == null) return BadRequest();

            if (sizeId == null) return BadRequest();

            if (count == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id &&
            p.ProductColors.Any(pc => pc.ColorId == colorId) &&
            p.ProductSizes.Any(ps => ps.SizeId == sizeId))) return NotFound();

            string? cookie = Request.Cookies["basket"];
            List<BasketVM>? basketVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                BasketVM vm = new BasketVM()
                {
                    Id = (int)id,
                    SizeId = (int)sizeId,
                    ColorId = (int)colorId,
                    Count = (int)count
                };

                basketVMs = new List<BasketVM> { vm };
            }

            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                if (basketVMs.Exists(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)))
                {
                    basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count += (int)count;
                }
                else
                {
                    BasketVM vm = new BasketVM()
                    {
                        Id = (int)id,
                        SizeId = (int)sizeId,
                        ColorId = (int)colorId,
                        Count = (int)count
                    };

                    basketVMs.Add(vm);
                }


            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (appUser.Baskets.Any(b => b.ProductId == id && (b.ColorId == colorId && b.SizeId == sizeId)))
                {
                    appUser.Baskets.FirstOrDefault(b => b.ProductId == id && (b.ColorId == colorId && b.SizeId == sizeId))
                        .Count += (int)count;
                }
                else
                {
                    Basket dbbasket = new Basket
                    {
                        ProductId = id,
                        Count = (int)count,
                        SizeId = (int)sizeId,
                        ColorId = (int)colorId,
                    };
                    appUser.Baskets.Add(dbbasket);
                }
                await _context.SaveChangesAsync();

            }

            string basket = JsonConvert.SerializeObject(basketVMs);

            Response.Cookies.Append("basket", basket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product? product = await _context.Products
                    .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                    .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Image = product.MainImage;
                    basketVM.Title = product.Title;
                    basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                    basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;
                    basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                }
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReview(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductCategories.Where(pc => pc.IsDeleted== false))
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                .Include(p=> p.ProductSizes.Where(pc=> pc.IsDeleted == false)).ThenInclude(pc => pc.Size)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.ProductId);

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid) return RedirectToAction("Detail", productReviewVM);

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Sign in to leave a review");
                return View(productReviewVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (productReviewVM != null && product.Reviews != null && product.Reviews.Any(r => r.UserId == appUser.Id))
            {
                ModelState.AddModelError("", "You have already wrote a review");
                return View(nameof(Detail), productReviewVM);
            }
            review.UserId = appUser.Id;
            review.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Detail), new { id = product.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Wishlist()
        {
            string? cookie = Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            IEnumerable<Product> reviewProducts = await _context.Products
                .Include(r => r.Reviews.Where(r => r.IsDeleted == false))
                .Where(rp => rp.IsDeleted == false)
                .ToListAsync();

            ViewBag.reviews = reviewProducts;

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

            return View(wishlistVMs);
        }

        public async Task<IActionResult> WishlistCount()
        {
            string? cookie = Request.Cookies["wishlist"];
            IEnumerable<WishlistVM>? wishlistVMs = null;

            IEnumerable<Product> reviewProducts = await _context.Products
                .Include(r => r.Reviews.Where(r => r.IsDeleted == false))
                .Where(rp => rp.IsDeleted == false)
                .ToListAsync();

            ViewBag.reviews = reviewProducts;

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

            return PartialView("_WishlistCount", wishlistVMs);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddWishlist(int? id)
        {
            if (id == null) return BadRequest();

            string? cookie = Request.Cookies["wishlist"];
            List<WishlistVM>? wishlistVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                WishlistVM vm = new WishlistVM()
                {
                    Id = (int)id
                };

                wishlistVMs = new List<WishlistVM> { vm };
            }

            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

                if (!wishlistVMs.Exists(b => b.Id == id))
                {
                    WishlistVM vm = new WishlistVM()
                    {
                        Id = (int)id
                    };
                    wishlistVMs.Add(vm);
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Wishlists.Where(w => w.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (!appUser.Wishlists.Any(b => b.ProductId == id))
                {
                    Wishlist dbWishlist = new Wishlist
                    {
                        ProductId = id,
                    };
                    appUser.Wishlists.Add(dbWishlist);
                }
                await _context.SaveChangesAsync();
            }

            string wishlist = JsonConvert.SerializeObject(wishlistVMs);

            Response.Cookies.Append("wishlist", wishlist);

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

            return PartialView("_WishlistCount",wishlistVMs);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DeletefromWishlist(int? id)
        {
            IEnumerable<Product> reviewProducts = await _context.Products
                .Include(r => r.Reviews.Where(r => r.IsDeleted == false))
                .Where(rp => rp.IsDeleted == false)
                .ToListAsync();

            ViewBag.reviews = reviewProducts;

            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string? cookie = Request.Cookies["wishlist"];

            if (string.IsNullOrWhiteSpace(cookie))
            {
                return BadRequest();
            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Wishlists)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                Wishlist wishlistToDelete = appUser.Wishlists.FirstOrDefault(b => b.ProductId == id);

                if (wishlistToDelete != null)
                {
                    appUser.Wishlists.Remove(wishlistToDelete);
                    _context.Wishlists.Remove(wishlistToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            List<WishlistVM> wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

            WishlistVM wishlistVM = wishlistVMs.Find(b => b.Id == id);

            if (wishlistVM == null) return NotFound();

            wishlistVMs.Remove(wishlistVM);

            cookie = JsonConvert.SerializeObject(wishlistVMs);
            HttpContext.Response.Cookies.Append("wishlist", cookie);

            foreach (WishlistVM item in wishlistVMs)
            {
                Product? product = await _context.Products
                        .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                        .Include(p => p.ProductSizes.Where(pc => pc.IsDeleted == false)).ThenInclude(ps => ps.Size)
                        .FirstOrDefaultAsync(p => p.Id == item.Id && p.IsDeleted == false);

                if (product != null)
                {
                    item.Product = product;
                    item.ProductId = product.Id;
                }
            }

            return PartialView("_WishlistPartial", wishlistVMs);
        }

        public async Task<IActionResult> UpdateProducts(int? id)
        {
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
                        wishlistVM.ProductId = product?.Id;
                    }
                }
            }

            Product product1 = await _context.Products
                 .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (wishlistVMs != null)
            {
                ViewBag.Wishlist = wishlistVMs;
            }

            return PartialView("_ProductLinkPartial",product1);
        }

        public async Task<IActionResult> UpdateDetailProducts(int? id)
        {
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
                        wishlistVM.ProductId = product?.Id;
                    }
                }
            }

            Product product1 = await _context.Products
                 .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (wishlistVMs != null)
            {
                ViewBag.Wishlist = wishlistVMs;
            }

            return PartialView("_ProductLinkDetailPartial", product1);
        }
    }
}
