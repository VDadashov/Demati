namespace Demati.Models
{
    public class GiftCard : BaseEntity
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
