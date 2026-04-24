using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;
using ActTogether.Helpers;

namespace ActTogether.Controllers
{
    // REST API for video posts — full CRUD, scoped to friends
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PostsApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/PostsApi
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendIds = await FriendHelper.GetFriendIds(_db, userId);
            friendIds.Add(userId);

            var posts = await _db.Posts
                .Where(p => friendIds.Contains(p.UserId))
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            return Ok(posts);
        }

        // GET: api/PostsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        // POST: api/PostsApi
        [HttpPost]
        public async Task<IActionResult> Post(Post post)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            post.UserId = userId;
            post.PostedAt = DateTime.Now;

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return Ok(post);
        }

        // UPDATE — only the post owner can edit
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Post updated)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var post = await _db.Posts.FindAsync(id);
            if (post == null || post.UserId != userId) return NotFound();

            post.Title = updated.Title;
            post.VideoUrl = updated.VideoUrl;
            post.Description = updated.Description;

            await _db.SaveChangesAsync();
            return Ok(post);
        }

        // DELETE — only the post owner can delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var post = await _db.Posts.FindAsync(id);
            if (post == null || post.UserId != userId) return NotFound();

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
