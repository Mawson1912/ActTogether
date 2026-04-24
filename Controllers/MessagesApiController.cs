using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // REST API for messages — inbox, conversations, send, mark read, delete
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MessagesApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/MessagesApi
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var messages = await _db.Messages
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

        // READ — only sender or receiver can view
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var message = await _db.Messages.FindAsync(id);

            if (message == null || (message.SenderId != userId && message.ReceiverId != userId))
                return NotFound();

            return Ok(message);
        }

        // GET: api/MessagesApi/conversation/5
        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversation(int otherUserId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var messages = await _db.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

        // POST: api/MessagesApi
        [HttpPost]
        public async Task<IActionResult> Post(Message message)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            message.SenderId = userId;
            message.SentAt = DateTime.Now;
            message.IsRead = false;

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return Ok(message);
        }

        // MARK READ — only the receiver can mark as read
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var message = await _db.Messages.FindAsync(id);

            if (message == null || message.ReceiverId != userId) return NotFound();

            message.IsRead = true;
            await _db.SaveChangesAsync();
            return Ok(message);
        }

        // DELETE: api/MessagesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var message = await _db.Messages.FindAsync(id);

            if (message == null || (message.SenderId != userId && message.ReceiverId != userId))
                return NotFound();

            _db.Messages.Remove(message);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
