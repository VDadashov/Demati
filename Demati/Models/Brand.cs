using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demati.Models
{
    public class Brand : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? FileImage { get; set; }
    }
}
