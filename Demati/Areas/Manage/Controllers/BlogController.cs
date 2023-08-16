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
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IEnumerable<Blog> blogs = _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            return View(PagenatedList<Blog>.Create(blogs, pageIndex, 4));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.BlogCategories = await _context.BlogCategories.Where(b => b.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ViewBag.BlogCategories = await _context.BlogCategories.Where(b => b.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(blog);

            if (blog.BlogCategoryId == null)
            {
                ModelState.AddModelError("BlogCategoryId", $"Must Be Selected");
                return View(blog);
            }

            if (!await _context.BlogCategories.AnyAsync(b => b.IsDeleted == false && b.Id == blog.BlogCategoryId))
            {
                ModelState.AddModelError("BlogCategoryId", $"BlogCategoryId: {blog.BlogCategoryId} - InCorrect");
                return View(blog);
            }

            if (blog.MainFile != null && !blog.MainFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainFile", "File Type Is InCorrect");
                return View(blog);
            }

            if (blog.MainFile != null && (blog.MainFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("MainFile", "File Size Can Be Max 500kb");
                return View(blog);
            }

            if (blog.MainFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + blog.MainFile.FileName.Substring(blog.MainFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Blog", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await blog.MainFile.CopyToAsync(fileStream);
                }

                blog.Image = fileName;
            }
            else
            {
                ModelState.AddModelError("MainFile", "Image Must be Selected");
                return View(blog);
            }

            blog.Title = blog.Title.Trim();
            blog.CreatedAt = DateTime.UtcNow.AddHours(4);
            blog.CreatedBy = $"{appUser.Name} {appUser.Surname}";

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (blog == null) return PartialView("_error-404");

            return View(blog);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Blog? blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (blog == null) return BadRequest();

            ViewBag.BlogCategories = await _context.BlogCategories.Where(b => b.IsDeleted == false).ToListAsync();

            return View(blog);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (id == null) return BadRequest();

            if (blog.Id != id) return BadRequest();

            Blog dbBlog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (blog.BlogCategoryId != null && !await _context.BlogCategories.AnyAsync(b => b.IsDeleted == false && b.Id == blog.BlogCategoryId))
            {
                ModelState.AddModelError("BlogCategoryId", $"BlogCategoryId: {blog.BlogCategoryId} - InCorrect");
                return View(blog);
            }

            if (blog.MainFile != null && !blog.MainFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainFile", "File Type Is InCorrect");
                return View(blog);
            }

            if (blog.MainFile != null && (blog.MainFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("MainFile", "File Size Can Be Max 500kb");
                return View(blog);
            }

            if (blog.MainFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + blog.MainFile.FileName.Substring(blog.MainFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Blog", dbBlog.Image);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Blog", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await blog.MainFile.CopyToAsync(fileStream);
                }

                dbBlog.Image = fileName;
            }

            dbBlog.Title = blog.Title.Trim();
            dbBlog.CreatedAt = DateTime.UtcNow.AddHours(4);
            dbBlog.CreatedBy = $"{appUser.Name} {appUser.Surname}";

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

            Blog blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (blog == null) return PartialView("_error-404");

            if (blog.Image != null)
            {
                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Blog", blog.Image);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            blog.IsDeleted = true;
            blog.DeletedBy = $"{appUser.Name} {appUser.Surname}";
            blog.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
