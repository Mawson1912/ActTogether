using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // Video post creation and deletion (MVC)
    [Authorize]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Post/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Post/Create
        [HttpPost]
        public async Task<IActionResult> Create(Post model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = new Post
            {
                UserId = userId,
                Title = model.Title,
                VideoUrl = model.VideoUrl,
                Description = model.Description,
                PostedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "User", new { id = userId });
        }

        // POST: /Post/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post != null)
            {
                _context.Feedbacks.RemoveRange(_context.Feedbacks.Where(f => f.PostId == id));
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyVideos");
        }

        // GET: /Post/MyVideos — shows own posts with private feedback
        public async Task<IActionResult> MyVideos(int page = 1)
        {
            const int pageSize = 10;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var totalPosts = await _context.Posts.Where(p => p.UserId == userId).CountAsync();

            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

            var postIds = posts.Select(p => p.Id).ToList();

            // Get feedback for all of user's posts
            var feedbacks = await _context.Feedbacks
                .Where(f => postIds.Contains(f.PostId))
                .OrderBy(f => f.CreatedAt)
                .ToListAsync();

            // Map sender IDs to usernames
            var senderIds = feedbacks.Select(f => f.SenderId).Distinct().ToList();
            ViewBag.SenderNames = await _context.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            // Group feedback by PostId for easy lookup in view
            ViewBag.FeedbackByPost = feedbacks.GroupBy(f => f.PostId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(posts);
        }

        // POST: /Post/GiveFeedback — friend leaves feedback on a post
        [HttpPost]
        public async Task<IActionResult> GiveFeedback(int postId, string content)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var feedback = new Feedback
            {
                PostId = postId,
                SenderId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
