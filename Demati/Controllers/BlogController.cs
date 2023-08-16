using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.BlogVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demati.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            BLogVM bLogVM = new BLogVM
            {
                BlogCategories = await _context.BlogCategories
                .Where(bc => bc.IsDeleted == false)
                .ToListAsync(),
                Beauty = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(bc => bc.IsDeleted == false && bc.BlogCategoryId == 1)
                .ToListAsync(),
                Entertainment = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(bc => bc.IsDeleted == false && bc.BlogCategoryId == 2)
                .ToListAsync(),
                Fashion = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(bc => bc.IsDeleted == false && bc.BlogCategoryId == 3)
                .ToListAsync(),
                Lifestyle = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(bc => bc.IsDeleted == false && bc.BlogCategoryId == 4)
                .ToListAsync(),
                Trending = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(bc => bc.IsDeleted == false && bc.BlogCategoryId == 5)
                .ToListAsync(),
            };


            return View(bLogVM);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            return View(blog);
        }
    }
}
