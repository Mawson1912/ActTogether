using Microsoft.EntityFrameworkCore;
using ActTogether.Data;

namespace ActTogether.Helpers
{
    // Shared helper to avoid duplicating friend ID lookup logic
    public static class FriendHelper
    {
        public static async Task<List<int>> GetFriendIds(AppDbContext db, int userId)
        {
            var friendships = await db.Friendships
                .Where(f => (f.User1Id == userId || f.User2Id == userId) && f.Status == "Accepted")
                .ToListAsync();

            return friendships
                .Select(f => f.User1Id == userId ? f.User2Id : f.User1Id)
                .ToList();
        }
    }
}
