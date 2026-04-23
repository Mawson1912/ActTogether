namespace ActTogether.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
