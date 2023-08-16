using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class OurTeam : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Job { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
        [StringLength(255)]
        public string Facebook { get; set; }
        [StringLength(255)]
        public string Instagram { get; set; }
        [StringLength(255)]
        public string Twitter { get; set; }
        [StringLength(255)]
        public string Youtube { get; set; }
    }
}
