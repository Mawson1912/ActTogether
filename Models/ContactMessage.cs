namespace ActTogether.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
