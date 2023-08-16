using System.ComponentModel.DataAnnotations;

namespace Demati.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(255)]
        public string SubTitle { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
        [StringLength(255)]
        public string Link { get; set; }
    }
}
