using Demati.Models;

namespace Demati.ViewModels.ShopVMs
{
    public class ShopVM
    {
        public IEnumerable<Color> Colors { get; set; }
        public IEnumerable<Size> Sizes { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
