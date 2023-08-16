using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class ProductImage : BaseEntity
    {
        [StringLength(100)]
        public string Image { get; set; }
        [StringLength(100)]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
