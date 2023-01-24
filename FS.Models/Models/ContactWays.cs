using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class ContactWays:BaseClass
    {
        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        public string Icon { get; set; }
        public bool IsLink { get; set; }
    }
}
