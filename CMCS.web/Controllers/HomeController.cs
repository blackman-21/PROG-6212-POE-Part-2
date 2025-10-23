using CMCS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Display all uploaded files
        public async Task<IActionResult> Uploads()
        {
            var files = await _db.UploadFiles
                .Include(f => f.Claim)
                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return View(files);
        }
    }
}
