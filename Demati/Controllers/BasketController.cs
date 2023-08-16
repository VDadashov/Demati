using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using System.Drawing;

namespace Demati.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddBasket(int? id, int? colorId, int? sizeId)
        {
            if (id == null) return BadRequest();

            if(colorId == null) return BadRequest();

            if (sizeId == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id &&
            p.ProductColors.Any(pc=>pc.ColorId == colorId) &&
            p.ProductSizes.Any(ps=>ps.SizeId == sizeId))) return NotFound();

            string? cookie = Request.Cookies["basket"];
            List<BasketVM>? basketVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                BasketVM vm = new BasketVM()
                {
                    Id = (int)id,
                    SizeId = (int)sizeId,
                    ColorId = (int)colorId,
                    Count = 1
                };

                basketVMs = new List<BasketVM> { vm };
            }

            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                if (basketVMs.Exists(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)))
                {
                    basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count += 1;
                }
                else
                {
                    BasketVM vm = new BasketVM()
                    {
                        Id = (int)id,
                        SizeId= (int)sizeId,
                        ColorId= (int)colorId,
                        Count = 1
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
                        .Count = basketVMs.FirstOrDefault(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count;
                }
                else
                {
                    Basket dbbasket = new Basket
                    {
                        ProductId = id,
                        Count = basketVMs.FirstOrDefault(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count,
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
                    .Include(p => p.ProductSizes).ThenInclude(ps =>ps.Size)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Image = product.MainImage;
                    basketVM.Title = product.Title;
                    basketVM.Color = product.ProductColors.FirstOrDefault(pc=>pc.ColorId == basketVM.ColorId).Color.Name;
                    basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;
                    basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                }
            }

            return PartialView("_BasketPartial", basketVMs);
        }
        [AllowAnonymous]
        public async Task<IActionResult> CartProductChangeCount(int? id, int? colorId, int? sizeId, int? count)
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
                    basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count = (int)count;
                }
            }

            string basket = JsonConvert.SerializeObject(basketVMs);

            Response.Cookies.Append("basket", basket);

            double subTotal = 0;

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

            return PartialView("_CartPartial", basketVMs);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshBasket(int? id, int? colorId, int? sizeId, int? count)
        {
            if (id == null) return BadRequest();

            if (colorId == null) return BadRequest();

            if (sizeId == null) return BadRequest();

            if(count == null) return BadRequest();

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
                    basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId)).Count = (int)count;
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
                        .Count = (int)count;
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
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshBasketCount()
        {
            string? cookie = Request.Cookies["basket"];
            List<BasketVM>? basketVMs = null;

            if(cookie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
            }

            return PartialView("_BasketCount", basketVMs);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DeletefromBasket(int? id, int? colorId, int? sizeId)
        {
            if (id == null) return BadRequest();

            if (colorId == null) return BadRequest();

            if (sizeId == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id &&
            p.ProductColors.Any(pc => pc.ColorId == colorId) &&
            p.ProductSizes.Any(ps => ps.SizeId == sizeId))) return NotFound();

            string? cookie = Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(cookie))
            {
                return BadRequest();
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            BasketVM basketVM = basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId));

            if (basketVM == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                Basket basketToDelete = appUser.Baskets.FirstOrDefault(b => b.ProductId == id && (b.ColorId == colorId && b.SizeId == sizeId));
                if (basketToDelete != null)
                {
                    appUser.Baskets.Remove(basketToDelete);
                    _context.Baskets.Remove(basketToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            basketVMs.Remove(basketVM);

            foreach (BasketVM item in basketVMs)
            {
                Product? product = await _context.Products
                    .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                    .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false)).ThenInclude(ps => ps.Size)
                    .FirstOrDefaultAsync(p => p.Id == item.Id && p.IsDeleted == false);

                if (product != null)
                {
                    item.Image = product.MainImage;
                    item.Title = product.Title;
                    item.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == item.ColorId).Color.Name;
                    item.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == item.SizeId).Size.Name;
                    item.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                }
            }

            cookie = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookie);

            return PartialView("_BasketPartial", basketVMs);
        }
        [AllowAnonymous]
        public async Task<IActionResult> DeletefromCart(int? id, int? colorId, int? sizeId)
        {
            if (id == null) return BadRequest();

            if (colorId == null) return BadRequest();

            if (sizeId == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id &&
            p.ProductColors.Any(pc => pc.ColorId == colorId) &&
            p.ProductSizes.Any(ps => ps.SizeId == sizeId))) return NotFound();

            string? cookie = Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(cookie))
            {
                return BadRequest();
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            BasketVM basketVM = basketVMs.Find(b => b.Id == id && (b.ColorId == colorId && b.SizeId == sizeId));

            if (basketVM == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                Basket basketToDelete = appUser.Baskets.FirstOrDefault(b => b.ProductId == id && (b.ColorId == colorId && b.SizeId == sizeId));
                if (basketToDelete != null)
                {
                    appUser.Baskets.Remove(basketToDelete);
                    _context.Baskets.Remove(basketToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            basketVMs.Remove(basketVM);

            double subTotal = 0;

            foreach (BasketVM item in basketVMs)
            {
                Product? product = await _context.Products
                    .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                    .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                    .FirstOrDefaultAsync(p => p.Id == item.Id && p.IsDeleted == false);

                if (product != null)
                {
                    item.Image = product.MainImage;
                    item.Title = product.Title;
                    item.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == item.ColorId).Color.Name;
                    item.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == item.SizeId).Size.Name;
                    item.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                }

                subTotal += (item.Price * item.Count);
            }

            cookie = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookie);

            ViewBag.subTotal = subTotal;

            return PartialView("_CartPartial", basketVMs);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetBasket()
        {
            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]));

        }

    }
}
