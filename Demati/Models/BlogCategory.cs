using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class BlogCategory : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
    }
}
