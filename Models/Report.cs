using System.ComponentModel.DataAnnotations;

namespace ActTogether.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int ReporterId { get; set; }
        public int ReportedUserId { get; set; }
        [MaxLength(500)]
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
