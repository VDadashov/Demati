using Demati.DataAccessLayer;
using Demati.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demati.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly AppDbContext _context;
        public ContactUsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Setting> settings = await _context.Settings
                .ToListAsync();

            return View(settings);
        }
    }
}
