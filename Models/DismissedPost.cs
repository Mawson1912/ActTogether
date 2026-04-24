namespace ActTogether.Models
{
    public class DismissedPost
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime DismissedAt { get; set; }
    }
}
