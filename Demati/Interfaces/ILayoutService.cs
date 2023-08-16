using Demati.Models;
using Demati.ViewModels.BasketVMs;
using Demati.ViewModels.ProductVMs;

namespace Demati.Interfaces
{
    public interface ILayoutService
    {
        Task<IEnumerable<Brand>> GetBrands();
        Task<IEnumerable<Setting>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBasket();
        Task<IEnumerable<WishlistVM>> GetWishlist();
    }
}
