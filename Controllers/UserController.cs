using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;
using Microsoft.AspNetCore.Http;

namespace ActTogether.Controllers
{
    // User profiles, editing, and search
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /User/Profile/5
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendshipStatus = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == currentUserId && f.User2Id == id) ||
                    (f.User1Id == id && f.User2Id == currentUserId));

            // Only show posts to the user themselves or confirmed friends
            bool isFriend = friendshipStatus != null && friendshipStatus.Status == "Accepted";
            var posts = new List<Post>();
            if (currentUserId == id || isFriend)
            {
                posts = await _context.Posts
                    .Where(p => p.UserId == id)
                    .OrderByDescending(p => p.PostedAt)
                    .ToListAsync();
            }

            ViewBag.Posts = posts;
            ViewBag.IsFriend = isFriend;
            ViewBag.FriendshipStatus = friendshipStatus;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsOwnProfile = currentUserId == id;

            return View(user);
        }

        // GET: /User/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /User/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(User model, IFormFile ProfileImage)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Bio = model.Bio;

            if (ProfileImage != null)
            {
                string[] allowedTypes = { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(ProfileImage.ContentType))
                {
                    ModelState.AddModelError("ProfileImage", "Only JPEG and PNG images are allowed");
                    return View(user);
                }

                if (ProfileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfileImage", "File size must be less than 2MB");
                    return View(user);
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(fileStream);
                }

                user.ProfileImageUrl = "/uploads/profiles/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", new { id = userId });
        }

        // GET: /User/Search
        public async Task<IActionResult> Search(string query)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var users = new List<User>();

            if (!string.IsNullOrEmpty(query))
            {
                users = await _context.Users
                    .Where(u => u.Id != currentUserId &&
                        (u.Username.Contains(query) || u.FullName.Contains(query)))
                    .ToListAsync();
            }

            // Lookup friendship status for each result
            var userIds = users.Select(u => u.Id).ToList();
            var friendships = await _context.Friendships
                .Where(f => (f.User1Id == currentUserId && userIds.Contains(f.User2Id))
                          || (f.User2Id == currentUserId && userIds.Contains(f.User1Id)))
                .ToListAsync();

            ViewBag.Friendships = friendships;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Query = query;
            return View(users);
        }

        // POST: /User/Report
        [HttpPost]
        public async Task<IActionResult> Report(int reportedUserId, string reason)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var report = new Report
            {
                ReporterId = currentUserId,
                ReportedUserId = reportedUserId,
                Reason = reason,
                CreatedAt = DateTime.Now
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Report submitted. Thank you.";
            return RedirectToAction("Profile", new { id = reportedUserId });
        }
    }
}
