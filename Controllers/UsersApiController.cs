using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // REST API for user data and unread message count
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersApiController(AppDbContext db)
        {
            _db = db;
        }

        // READ ALL — excludes password hash from response
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _db.Users.Select(u => new
            {
                u.Id,
                u.Username,
                u.FullName,
                u.Bio,
                u.CreatedAt
            }).ToListAsync();

            return Ok(users);
        }

        // GET: api/UsersApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _db.Users
                .Where(u => u.Id == id)
                .Select(u => new { u.Id, u.Username, u.FullName, u.Bio, u.CreatedAt })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // UPDATE — users can only edit their own profile
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, User updated)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != userId) return Unauthorized();

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = updated.FullName;
            user.Bio = updated.Bio;

            await _db.SaveChangesAsync();
            return Ok(new { user.Id, user.Username, user.FullName, user.Bio, user.CreatedAt });
        }

        // Used by JS in _Layout to show unread badge
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var count = await _db.Messages.CountAsync(m => m.ReceiverId == userId && !m.IsRead);
            return Ok(new { unreadCount = count });
        }
    }
}
