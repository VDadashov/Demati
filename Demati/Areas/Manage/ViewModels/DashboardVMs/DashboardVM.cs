using Demati.Models;

namespace Demati.Areas.Manage.ViewModels.DashboardVMs
{
    public class DashboardVM
    {
        public IEnumerable<Product> LastRemainingProducts { get; set; }
        public IEnumerable<Product> LatestUpdatedProducts { get; set; }
    }
}
