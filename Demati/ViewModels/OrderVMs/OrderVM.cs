using Demati.Models;
using Demati.ViewModels.BasketVMs;

namespace Demati.ViewModels.OrderVMs
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}
