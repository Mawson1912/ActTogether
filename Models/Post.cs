using System.ComponentModel.DataAnnotations;

namespace ActTogether.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }
        public User User { get; set; }
    }
}
