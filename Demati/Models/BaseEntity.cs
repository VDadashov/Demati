using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(255)]
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
