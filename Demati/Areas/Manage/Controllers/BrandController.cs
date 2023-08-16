using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Metadata;

namespace Demati.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public BrandController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IEnumerable<Brand> brands = _context.Brands
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            return View(PagenatedList<Brand>.Create(brands, pageIndex, 4));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", "Already Exist");
                return View();
            }

            if (brand.FileImage != null)
            {
                if (!brand.FileImage.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("FileImage", "File Type Is InCorrect");
                    return View(brand);
                }

                if ((brand.FileImage.Length / 1024) > 500)
                {
                    ModelState.AddModelError("FileImage", "File Size Can Be Max 500kb");
                    return View(brand);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + brand.FileImage.FileName.Substring(brand.FileImage.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath,"assets", "images", "Brands", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await brand.FileImage.CopyToAsync(fileStream);
                }

                brand.Image = fileName;
            }
            else
            {
                ModelState.AddModelError("FileImage", "Image Must be Selected");
                return View(brand);
            }


            brand.Name = brand.Name.Trim();
            brand.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            brand.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (brand == null) return PartialView("_error-404");

            return View(brand);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Brand brand)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (!ModelState.IsValid) return View(brand);

            if (id == null) return BadRequest();
            if (brand.Id != id) return BadRequest();

            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);
            
            if (dbBrand == null) return PartialView("_error-404");

            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.ToLower().Trim() && b.Id != id))
            {
                ModelState.AddModelError("Name", "Already Exist");
                return View();
            }

            if (brand.FileImage != null)
            {
                if (!brand.FileImage.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("FileImage", "File Type Is InCorrect");
                    return View(brand);
                }

                if ((brand.FileImage.Length / 1024) > 500)
                {
                    ModelState.AddModelError("FileImage", "File Size Can Be Max 500kb");
                    return View(brand);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + brand.FileImage.FileName.Substring(brand.FileImage.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Brands", dbBrand.Image);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Blog", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await brand.FileImage.CopyToAsync(fileStream);
                }

                dbBrand.Image = fileName;
            }

            dbBrand.Name = brand.Name.Trim();
            dbBrand.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbBrand.UpdatedBy = $"{appUser.Name} {appUser.Surname}";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return PartialView("_error-404");

            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "Brands", brand.Image);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            brand.IsDeleted = true;
            brand.DeletedBy = $"{appUser.Name} {appUser.Surname}";
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
