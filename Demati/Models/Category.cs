using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demati.Models
{
    public class Category : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        public IEnumerable<ProductCategory>? ProductCategories { get; set; }

    }
}
