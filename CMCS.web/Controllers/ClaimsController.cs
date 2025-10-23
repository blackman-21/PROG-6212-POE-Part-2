﻿using CMCS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;



namespace CMCS.Web.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public ClaimsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _config = config;
        }

        // Lecturer view: list all claims (in production filter by lecturer)
        public async Task<IActionResult> Index()
        {
            var list = await _db.Claims.Include(c => c.Uploads).OrderByDescending(c => c.SubmittedAt).ToListAsync();
            return View(list);
        }

        // Create form GET
        public IActionResult Create()
        {
            return View(new Claim { LecturerName = "John Doe" });
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LecturerName,Month,TotalHours,HourlyRate,Notes")] Claim claim, List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            // Save claim to get ID
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uploadSection = _config.GetSection("UploadSettings");
            long maxBytes = uploadSection.GetValue<long>("MaxFileBytes", 5242880);
            string[] allowed = uploadSection.GetSection("AllowedExtensions").Get<string[]>()
                   ?? new[] { ".pdf", ".docx", ".xlsx" };


            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                if (file.Length > maxBytes)
                {
                    ModelState.AddModelError(string.Empty, $"File {file.FileName} exceeds max size of {maxBytes} bytes.");
                    continue;
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, $"File {file.FileName} has invalid file type.");
                    continue;
                }

                var savedName = $"{Guid.NewGuid()}{ext}";
                var savePath = Path.Combine(uploadsFolder, savedName);
                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                _db.UploadFiles.Add(new UploadFile
                {
                    ClaimId = claim.Id,
                    FileName = savedName,
                    OriginalFileName = file.FileName,
                    Size = file.Length
                });
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Claim details
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _db.Claims.Include(c => c.Uploads).FirstOrDefaultAsync(c => c.Id == id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        // Coordinator/Manager: view pending claims
        public async Task<IActionResult> Pending()
        {
            var pending = await _db.Claims.Where(c => c.Status == ClaimStatus.Pending).Include(c => c.Uploads).OrderBy(c => c.SubmittedAt).ToListAsync();
            return View(pending);
        }

        // Approve a claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Approved;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Claim {id} approved.";
            return RedirectToAction(nameof(Pending));
        }

        // Reject a claim (reason optional)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Rejected;
            claim.Notes = (claim.Notes ?? "") + $"\n[Rejected at {DateTime.UtcNow:u}] Reason: {reason}";
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Claim {id} rejected.";
            return RedirectToAction(nameof(Pending));
        }

        // Download a stored file
        public async Task<IActionResult> Download(int id)
        {
            var file = await _db.UploadFiles.FindAsync(id);
            if (file == null) return NotFound();

            var path = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", file.FileName);
            if (!System.IO.File.Exists(path)) return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(path, contentType, file.OriginalFileName);
        }
    }
}
