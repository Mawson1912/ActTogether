using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // REST API for friendships — list, requests, accept/reject, remove
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendshipsApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FriendshipsApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/FriendshipsApi
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendships = await _db.Friendships
                .Where(f => (f.User1Id == userId || f.User2Id == userId) && f.Status == "Accepted")
                .ToListAsync();

            var friendIds = friendships
                .Select(f => f.User1Id == userId ? f.User2Id : f.User1Id)
                .ToList();

            var friends = await _db.Users
                .Where(u => friendIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Username, u.FullName })
                .ToListAsync();

            return Ok(friends);
        }

        // GET: api/FriendshipsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var friendship = await _db.Friendships.FindAsync(id);
            if (friendship == null) return NotFound();
            return Ok(friendship);
        }

        // GET: api/FriendshipsApi/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var pending = await _db.Friendships
                .Where(f => f.User2Id == userId && f.Status == "Pending" && f.RequestedBy != userId)
                .ToListAsync();

            return Ok(pending);
        }

        // Used by JS in _Layout to show pending request badge
        [HttpGet("pendingcount")]
        public async Task<IActionResult> GetPendingCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var count = await _db.Friendships
                .CountAsync(f => f.User2Id == userId && f.Status == "Pending" && f.RequestedBy != userId);

            return Ok(new { pendingCount = count });
        }

        // POST: api/FriendshipsApi
        [HttpPost]
        public async Task<IActionResult> Post(Friendship friendship)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var existing = await _db.Friendships.FirstOrDefaultAsync(f =>
                (f.User1Id == userId && f.User2Id == friendship.User2Id) ||
                (f.User1Id == friendship.User2Id && f.User2Id == userId));

            if (existing != null) return Ok(existing); // Prevent duplicate requests

            friendship.User1Id = userId;
            friendship.Status = "Pending";
            friendship.RequestedBy = userId;
            friendship.CreatedAt = DateTime.Now;

            _db.Friendships.Add(friendship);
            await _db.SaveChangesAsync();
            return Ok(friendship);
        }

        // UPDATE — accept or reject (only the recipient can change status)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Friendship updated)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendship = await _db.Friendships.FindAsync(id);

            if (friendship == null || friendship.User2Id != userId) return NotFound();

            friendship.Status = updated.Status;
            await _db.SaveChangesAsync();
            return Ok(friendship);
        }

        // DELETE: api/FriendshipsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendship = await _db.Friendships.FindAsync(id);

            if (friendship == null ||
                (friendship.User1Id != userId && friendship.User2Id != userId))
                return NotFound();

            _db.Friendships.Remove(friendship);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
