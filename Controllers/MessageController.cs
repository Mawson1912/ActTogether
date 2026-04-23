using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;

namespace ActTogether.Controllers
{
    // Inbox, conversations, and sending messages (MVC)
    [Authorize]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Message/Inbox — grouped by conversation partner
        public async Task<IActionResult> Inbox()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Get all messages involving this user
            var allMessages = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            // Group by conversation partner
            var conversations = allMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new {
                    OtherUserId = g.Key,
                    LatestMessage = g.First(),
                    UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
                })
                .OrderByDescending(c => c.LatestMessage.SentAt)
                .ToList();

            var otherUserIds = conversations.Select(c => c.OtherUserId).ToList();
            var userNames = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            ViewBag.UserNames = userNames;
            ViewBag.Conversations = conversations.Select(c => new {
                c.OtherUserId,
                Preview = c.LatestMessage.Content.Length > 60
                    ? c.LatestMessage.Content.Substring(0, 60) + "..."
                    : c.LatestMessage.Content,
                c.LatestMessage.SentAt,
                c.UnreadCount,
                IsFromMe = c.LatestMessage.SenderId == userId
            }).ToList();

            return View();
        }

        // GET: /Message/Conversation/5
        public async Task<IActionResult> Conversation(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (otherUser == null) return NotFound();

            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == id) ||
                    (m.SenderId == id && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Mark unread messages as read
            var unread = messages.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();
            foreach (var msg in unread)
            {
                msg.IsRead = true;
            }
            if (unread.Any()) await _context.SaveChangesAsync();

            ViewBag.OtherUser = otherUser;
            ViewBag.CurrentUserId = userId;
            return View(messages);
        }

        // POST: /Message/Send
        [HttpPost]
        public async Task<IActionResult> Send(int receiverId, string content)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Conversation", new { id = receiverId });
        }
    }
}
