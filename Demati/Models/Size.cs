using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class Size : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }

        public IEnumerable<ProductSize>? ProductSizes { get; set; }
    }
}
