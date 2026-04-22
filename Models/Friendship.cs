namespace ActTogether.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public string Status { get; set; } // Pending, Accepted, Rejected, Blocked
        public int RequestedBy { get; set; }
        public int? BlockedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
