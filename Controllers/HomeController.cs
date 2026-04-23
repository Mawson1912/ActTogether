using ActTogether.Models;
using ActTogether.Data;
using ActTogether.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;

namespace ActTogether.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendIds = await FriendHelper.GetFriendIds(_context, userId);

            // Feed shows friends' posts only
            var posts = await _context.Posts
                .Where(p => friendIds.Contains(p.UserId))
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            // Map user IDs to usernames for display in feed
            var userNames = await _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            // Track which posts this user has already given feedback on
            var postIds = posts.Select(p => p.Id).ToList();
            var reviewedPostIds = await _context.Feedbacks
                .Where(f => f.SenderId == userId && postIds.Contains(f.PostId))
                .Select(f => f.PostId)
                .Distinct()
                .ToListAsync();

            ViewBag.UserNames = userNames;
            ViewBag.ReviewedPostIds = new HashSet<int>(reviewedPostIds);
            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ContactUs(string message)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var contactMessage = new ContactMessage
            {
                UserId = userId,
                Message = message,
                SentAt = DateTime.Now
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            ViewBag.Submitted = true;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
