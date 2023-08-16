using System.Buffers.Text;

namespace Demati.Models
{
    public class Basket:BaseEntity
    {
        public int Count { get; set; }
        public int? SizeId { get; set; }
        public Size? Size { get; set; }
        public int? ColorId { get; set; }
        public Color? Color { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
