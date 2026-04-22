using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

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
        public async Task<IActionResult> Edit(User model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Bio = model.Bio;
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

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Query = query;
            return View(users);
        }
    }
}
