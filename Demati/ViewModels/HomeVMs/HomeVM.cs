using Demati.Models;

namespace Demati.ViewModels.HomeVMs
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Product> Featured { get; set; }
        public IEnumerable<Product> NewArrivals { get; set; }
        public IEnumerable<Product> Bestseller { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
    }
}
