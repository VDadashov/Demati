using Demati.Models;

namespace Demati.ViewModels.AboutUsVMs
{
    public class AboutUsVM
    {
        public IEnumerable<OurTeam> OurTeams { get; set; }
        public IEnumerable<Setting> Settings { get; set; }
    }
}
