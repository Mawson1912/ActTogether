using System.ComponentModel.DataAnnotations;

namespace ActTogether.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int SenderId { get; set; }
        [MaxLength(2000)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
