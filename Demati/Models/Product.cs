using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(255)]
        public string Info { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DisCountPrice { get; set; }
        [Range(0, int.MaxValue)]
        public int Count { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNewArrivals { get; set; }
        public bool IsBestseller { get; set; }
        public bool isNewIn { get; set; }
        public bool isHot { get; set; }
        [StringLength(100)]
        public string? HoverImage { get; set; }
        [StringLength(100)]
        public string? MainImage { get; set; }

        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }

        [NotMapped]
        public IFormFile? HoverFile { get; set; }
        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Images { get; set; }
        [NotMapped]
        public IEnumerable<int>? CategoryIds { get; set; }
        [NotMapped]
        public IEnumerable<int>? ColorIds { get; set; }
        [NotMapped]
        public IEnumerable<int>? SizeIds { get; set; }

    }
}
