using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Demati.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CategoryController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IEnumerable<Category> categories = _context.Categories
                .Include(c=>c.ProductCategories)
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            return View(PagenatedList<Category>.Create(categories, pageIndex, 5));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? id, int pageIndex = 1)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.ProductCategories).ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            ViewBag.Name = category.Name;

            if (category == null) return PartialView("_error-404");

            IEnumerable<Product> products = _context.Products
                .Include(p => p.ProductCategories)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            if (products == null) return PartialView("_error-404");

            products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == id));

            return View(PagenatedList<Product>.Create(products, pageIndex, 5));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", $"Category Name: {category.Name.Trim()} Already Exist");
                return View(category);
            }

            category.Name = category.Name.Trim();
            category.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return PartialView("_error-404");

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (!ModelState.IsValid) return View(category);

            if (id == null) return BadRequest();

            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (dbCategory == null) return PartialView("_error-404");

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("Name", "Same Name Already Exists");
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCategory.UpdatedBy = $"{appUser.Name} {appUser.Surname}"; ;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id, int pageIndex = 1)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.ProductCategories).ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            ViewBag.Name = category.Name;
            ViewBag.Id = category.Id;

            if (category == null) return PartialView("_error-404");

            IEnumerable<Product> products = _context.Products
                .Include(p => p.ProductCategories)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            if (products == null) return PartialView("_error-404");

            products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == id));

            return View(PagenatedList<Product>.Create(products, pageIndex, 5));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.ProductCategories).ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return PartialView("_error-404");

            category.IsDeleted = true;
            category.DeletedBy = $"{appUser.Name} {appUser.Surname}"; ;
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            foreach (ProductCategory productCategory in category.ProductCategories)
            {
                productCategory.Product.IsDeleted = true;
                productCategory.Product.DeletedBy = $"{appUser.Name} {appUser.Surname}"; ;
                productCategory.Product.DeletedAt = DateTime.UtcNow.AddHours(4);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
