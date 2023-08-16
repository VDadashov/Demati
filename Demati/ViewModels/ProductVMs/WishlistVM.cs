using Demati.Models;

namespace Demati.ViewModels.ProductVMs
{
    public class WishlistVM
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
