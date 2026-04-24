using System.ComponentModel.DataAnnotations.Schema;

namespace ActTogether.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
