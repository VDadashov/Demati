using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.BasketVMs;
using Demati.ViewModels.OrderVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Demati.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public OrderController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {
            string cookie = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                RedirectToAction("Index", "Shop");
            }

            double subTotal = 0;
            double shipping = 0;

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products
                    .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false && ps.SizeId == basketVM.SizeId)).ThenInclude(ps => ps.Size)
                    .Include(p => p.ProductColors.Where(ps => ps.IsDeleted == false && ps.ColorId == basketVM.ColorId)).ThenInclude(pc => pc.Color)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                basketVM.Title = product.Title;
                basketVM.Image = product.MainImage;
                basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;

                subTotal += basketVM.Price * basketVM.Count;
                shipping += basketVM.Count * 2.5;
            }

            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            Order order = new Order
            {
                Name = appUser.Name,
                Surname = appUser.Surname,
                Email = appUser.Email,
                Phone = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.Phone,
                AddressLine = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.AddressLine,
                City = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.City,
                Country = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.Country,
                PostalCode = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.PostalCode,
                Company = appUser.Addresses?.FirstOrDefault(a => a.IsMain)?.Company,
            };

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs
            };

            shipping = subTotal > 150 ? 0 : shipping;

            ViewBag.subTotal = subTotal;
            ViewBag.shipping = shipping;

            return View(orderVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
               .Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
               .Include(u => u.Orders)
               .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
               .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser.Addresses == null && appUser.Addresses.Count() <= 0)
            {
                ModelState.AddModelError("", "You Need to Add Address First before Ordering");
                return View(order);
            }
            string cookie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(cookie))
            {
                RedirectToAction("Index", "Shop");
            }

            double subTotal = 0;
            int? statusCheck = null;

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products
                    .Include(p =>p.ProductSizes.Where(ps => ps.IsDeleted == false && ps.SizeId == basketVM.SizeId)).ThenInclude(ps => ps.Size)
                    .Include(p =>p.ProductColors.Where(ps => ps.IsDeleted == false && ps.ColorId == basketVM.ColorId)).ThenInclude(pc => pc.Color)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                basketVM.Title = product.Title;
                basketVM.Image = product.MainImage;
                basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;
                product.Count = (product.Count - basketVM.Count) < 0 ? product.Count : product.Count - basketVM.Count;
                statusCheck = (product.Count - basketVM.Count) < 0 ? 2 : 0;
                order.Status = (Enums.OrderType)statusCheck;
                order.Comment = statusCheck == 2 ? "OutOfStock" : "";

                subTotal += basketVM.Price * basketVM.Count;

                basketVM.Price = basketVM.Price - (basketVM.Price * (basketVM.DiscountPercent != null ? basketVM.DiscountPercent : 0) / 100);

                await _context.SaveChangesAsync();

            }
            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs
            };

            if (!ModelState.IsValid) return View(orderVM);

            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (BasketVM basketVM1 in basketVMs)
            {
                OrderItem orderItem = new OrderItem
                {
                    Count = basketVM1.Count,
                    ProductId = basketVM1.Id,
                    Price = basketVM1.Price,
                    Color = basketVM1.Color,
                    Size = basketVM1.Size,
                    Shipping = subTotal > 150 ? 0 : basketVM1.Count * 2.5,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.Surname}"
                };
                orderItems.Add(orderItem);

            };

            List<Basket> basketsToDelete = new List<Basket>();

            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;
                basketsToDelete.Add(basket);
            }

            foreach (Basket basketToDelete in basketsToDelete)
            {
                appUser.Baskets.Remove(basketToDelete);
                _context.Baskets.Remove(basketToDelete);
            }
            HttpContext.Response.Cookies.Append("basket", "");

            order.UserId = appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            order.OrderItems = orderItems;

            order.No = appUser.Orders != null && appUser.Orders.Count() > 0 ? appUser.Orders.Last().No + 1 : 1;


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GiftCard(string? giftCode,string? shipping1, string? subTotal1)
        {
            if (giftCode == null)
            {
                TempData["ErrorMessage"] = "Gift code is not available";
                ViewBag.subTotal = subTotal1;
                ViewBag.shipping = shipping1;
                return View();
            }

            GiftCard giftCard = await _context.Giftcards
                .FirstOrDefaultAsync(g => g.IsDeleted == false && g.Code.ToLower() == giftCode.Trim().ToLower());

            if (giftCard == null)
            {
                TempData["ErrorMessage"] = "Gift code is not available";
                ViewBag.subTotal = subTotal1;
                ViewBag.shipping = shipping1;
                return PartialView("_GiftCardPartial", giftCard);
            }

            if (giftCard.ExpirationDate?.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Gift code has expired";
                ViewBag.subTotal = subTotal1;
                ViewBag.shipping = shipping1;
                return PartialView("_GiftCardPartial", giftCard);
            }

            string cookie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(cookie))
            {
                RedirectToAction("Index", "Shop");
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            double subTotal = 0;
            double shipping = 0;
            double discountPrice = 0;

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products
                    .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false && ps.SizeId == basketVM.SizeId)).ThenInclude(ps => ps.Size)
                    .Include(p => p.ProductColors.Where(ps => ps.IsDeleted == false && ps.ColorId == basketVM.ColorId)).ThenInclude(pc => pc.Color)
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                basketVM.Title = product.Title;
                basketVM.Image = product.MainImage;
                basketVM.Color = product.ProductColors.FirstOrDefault(pc => pc.ColorId == basketVM.ColorId).Color.Name;
                basketVM.Size = product.ProductSizes.FirstOrDefault(ps => ps.SizeId == basketVM.SizeId).Size.Name;

                subTotal += basketVM.Price * basketVM.Count;
                shipping += basketVM.Count * 2.5;

                basketVM.DiscountPercent = (double)giftCard.DiscountPercent;
                basketVM.GiftCode = giftCard.Code;

                discountPrice += basketVM.Price * basketVM.DiscountPercent / 100;
                basketVM.Price = basketVM.Price - (basketVM.Price * basketVM.DiscountPercent / 100);
            }

            string basket = JsonConvert.SerializeObject(basketVMs);

            Response.Cookies.Append("basket", basket);

            ViewBag.subTotal = subTotal;
            ViewBag.shipping = shipping;
            ViewBag.discountPrice = discountPrice;

            return PartialView("_GiftCardPartial", giftCard);
        }
    }
}
