using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demati.Models
{
    public class Blog : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        public int? BlogCategoryId { get; set; }
        public BlogCategory? BlogCategory { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }
    }
}
