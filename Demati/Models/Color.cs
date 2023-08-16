using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class Color : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }

        public IEnumerable<ProductColor>? ProductColors { get; set; }
    }
}
