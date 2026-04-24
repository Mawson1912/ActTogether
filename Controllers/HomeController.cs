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
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendIds = await FriendHelper.GetFriendIds(_context, userId);

            // Get IDs of posts this user has dismissed
            var dismissedIds = await _context.DismissedPosts
                .Where(d => d.UserId == userId)
                .Select(d => d.PostId)
                .ToListAsync();

            // Feed shows friends' posts only, excluding dismissed
            var totalPosts = await _context.Posts
                .Include(p => p.User)
                .Where(p => friendIds.Contains(p.UserId) && !dismissedIds.Contains(p.Id))
                .CountAsync();

            var posts = await _context.Posts
                .Include(p => p.User)
                .Where(p => friendIds.Contains(p.UserId) && !dismissedIds.Contains(p.Id))
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
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
