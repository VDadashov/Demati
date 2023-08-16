using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels.AboutUsVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demati.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly AppDbContext _context;
        public AboutUsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            AboutUsVM aboutUsVM = new AboutUsVM
            {
                OurTeams = await _context.OurTeams
                .Where(ot => ot.IsDeleted == false)
                .ToListAsync(),
                Settings = await _context.Settings
                .ToListAsync()
        };

            return View(aboutUsVM);
        }
    }
}
