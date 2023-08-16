using Demati.Models;

namespace Demati.ViewModels.BasketVMs
{
    public class BasketVM
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public int? ColorId { get; set; }
        public string Color { get; set; }
        public int? SizeId { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string? GiftCode { get; set; }
        public double DiscountPercent { get; set; }
    }
}
