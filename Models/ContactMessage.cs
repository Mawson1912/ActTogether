using System.ComponentModel.DataAnnotations;

namespace ActTogether.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(2000)]
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
