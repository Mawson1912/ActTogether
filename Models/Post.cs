using System.ComponentModel.DataAnnotations;

namespace ActTogether.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string VideoUrl { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }
        public User User { get; set; }
    }
}
