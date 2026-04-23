using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // Admin dashboard — restricted to users with Admin role
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: /Admin/ContactMessages
        public async Task<IActionResult> ContactMessages()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var userIds = messages.Select(m => m.UserId).Distinct().ToList();
            ViewBag.UserNames = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            return View(messages);
        }

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var reports = await _context.Reports
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // Map user IDs to usernames for display
            var userIds = reports.Select(r => r.ReporterId)
                .Union(reports.Select(r => r.ReportedUserId))
                .Distinct().ToList();

            ViewBag.UserNames = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            return View(reports);
        }

        // POST: /Admin/DeleteUser/5 — removes user and all their data
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (id == currentUserId)
            {
                TempData["Message"] = "You cannot remove your own account from here. Please use Edit Profile or escalate to another admin.";
                return RedirectToAction("Reports");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Remove all related data
            _context.Messages.RemoveRange(_context.Messages.Where(m => m.SenderId == id || m.ReceiverId == id));
            _context.Posts.RemoveRange(_context.Posts.Where(p => p.UserId == id));
            _context.Friendships.RemoveRange(_context.Friendships.Where(f => f.User1Id == id || f.User2Id == id));
            _context.ContactMessages.RemoveRange(_context.ContactMessages.Where(c => c.UserId == id));
            _context.Reports.RemoveRange(_context.Reports.Where(r => r.ReporterId == id || r.ReportedUserId == id));
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            TempData["Message"] = $"User '{user.Username}' has been removed.";
            return RedirectToAction("Reports");
        }
    }
}
