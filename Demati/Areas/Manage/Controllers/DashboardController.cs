using Demati.Areas.Manage.ViewModels.DashboardVMs;
using Demati.DataAccessLayer;
using Demati.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Demati.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index()
        {
            //the least remaining products
            IEnumerable<Product> products = await _context.Products
                .Where(p => p.IsDeleted == false && p.Count > 0)
                .OrderBy(p => p.Count)
                .ToListAsync();

            //number of discounted products
            IEnumerable<Product> discountedProducts = await _context.Products
                .Where(p => p.IsDeleted == false && p.DisCountPrice > 0)
                .ToListAsync();

            //latest updated products
            IEnumerable<Product> latestUpdatedProducts = await _context.Products
                .Where(p => p.IsDeleted == false && p.UpdatedAt != null)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            IEnumerable<Order> orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.IsDeleted == false)
                .ToListAsync();

            int productCount = 0;


            foreach (Product product in products)
            {
                productCount += product.Count;
            }

            ViewBag.AllUsers = await _context.Users.ToListAsync();
            ViewBag.AllProduct = _context.Products.Count();
            ViewBag.ProductCount = productCount;
            ViewBag.DiscountedProducts = discountedProducts.Count();

            ViewBag.monthIncome = orders.Where(o => o.CreatedAt.Month == DateTime.Today.Month);

            ViewBag.FirstMonthCount = orders.Where(o => o.CreatedAt.Month == DateTime.Today.AddMonths(-4).Month).Count();
            ViewBag.FirstMonth = DateTime.Today.AddMonths(-4).ToString("MMM");

            ViewBag.SecondMonthCount = orders.Where(o => o.CreatedAt.Month == DateTime.Today.AddMonths(-3).Month).Count();
            ViewBag.SecondMonth = DateTime.Today.AddMonths(-3).ToString("MMM");

            ViewBag.ThirdMonthCount = orders.Where(o => o.CreatedAt.Month == DateTime.Today.AddMonths(-2).Month).Count();
            ViewBag.ThirdMonth = DateTime.Today.AddMonths(-2).ToString("MMM");

            ViewBag.FourthMonthCount = orders.Where(o => o.CreatedAt.Month == DateTime.Today.AddMonths(-1).Month).Count();
            ViewBag.FourthMonth = DateTime.Today.AddMonths(-1).ToString("MMM");

            ViewBag.FifthMonthCount = orders.Where(o => o.CreatedAt.Month == DateTime.Today.Month).Count();
            ViewBag.FifthMonth = DateTime.Today.ToString("MMM");




            DashboardVM dashboardVM = new DashboardVM
            {
                LastRemainingProducts = products.Take(7).ToList(),
                LatestUpdatedProducts = latestUpdatedProducts.Take(7).ToList(),
            };

            return View(dashboardVM);
        }
    }
}
