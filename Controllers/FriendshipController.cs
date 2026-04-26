using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActTogether.Data;
using ActTogether.Models;
using ActTogether.Helpers;

namespace ActTogether.Controllers
{
    // Friend requests, acceptance, rejection, and removal
    [Authorize]
    public class FriendshipController : Controller
    {
        private readonly AppDbContext _context;

        public FriendshipController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Friendship/Friends
        public async Task<IActionResult> Friends()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendIds = await FriendHelper.GetFriendIds(_context, userId);
            var friends = await _context.Users.Where(u => friendIds.Contains(u.Id)).ToListAsync();

            return View(friends);
        }

        // GET: /Friendship/Requests
        public async Task<IActionResult> Requests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var pending = await _context.Friendships
                .Where(f => f.User2Id == userId && f.Status == "Pending" && f.RequestedBy != userId)
                .ToListAsync();

            var requesterIds = pending.Select(f => f.User1Id).ToList();
            var requesters = await _context.Users.Where(u => requesterIds.Contains(u.Id)).ToListAsync();

            ViewBag.Friendships = pending;
            return View(requesters);
        }

        // POST: /Friendship/SendRequest/5
        [HttpPost]
        public async Task<IActionResult> SendRequest(int id, string returnUrl = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var existing = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.User1Id == userId && f.User2Id == id) ||
                (f.User1Id == id && f.User2Id == userId));

            // Prevent sending request if blocked by the other user
            if (existing != null)
            {
                if (existing.Status == "Blocked" && existing.BlockedBy != userId)
                    TempData["Message"] = "Unable to send friend request.";
                return Redirect(returnUrl ?? Url.Action("Profile", "User", new { id }));
            }

            var friendship = new Friendship
            {
                User1Id = userId,
                User2Id = id,
                Status = "Pending",
                RequestedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Friend request sent";

            return Redirect(returnUrl ?? Url.Action("Profile", "User", new { id }));
        }

        // POST: /Friendship/Accept/5
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id);

            if (friendship != null && friendship.User2Id == userId)
            {
                friendship.Status = "Accepted";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Requests");
        }

        // POST: /Friendship/Reject/5
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id);

            if (friendship != null && friendship.User2Id == userId)
            {
                friendship.Status = "Rejected";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Requests");
        }

        // POST: /Friendship/Remove/5 — sets status to Blocked
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.User1Id == userId && f.User2Id == id) ||
                (f.User1Id == id && f.User2Id == userId));

            if (friendship != null)
            {
                friendship.Status = "Blocked";
                friendship.BlockedBy = userId;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Friends");
        }

        // POST: /Friendship/Unblock/5 — deletes the row so re-friending is possible
        [HttpPost]
        public async Task<IActionResult> Unblock(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.User1Id == userId && f.User2Id == id) ||
                (f.User1Id == id && f.User2Id == userId));

            if (friendship != null && friendship.BlockedBy == userId)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "User", new { id });
        }
    }
}
