using Demati.Models;

namespace Demati.ViewModels.BlogVMs
{
    public class BLogVM
    {
        public IEnumerable<BlogCategory> BlogCategories { get; set; }
        public IEnumerable<Blog> Beauty { get; set; }
        public IEnumerable<Blog> Entertainment { get; set; }
        public IEnumerable<Blog> Fashion { get; set; }
        public IEnumerable<Blog> Lifestyle { get; set; }
        public IEnumerable<Blog> Trending { get; set; }

    }
}
