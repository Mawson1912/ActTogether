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
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyVideos");
        }

        // GET: /Post/MyVideos — shows own posts (basic version, no feedback yet)
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

            return View(posts);
        }
    }
}
