using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class BenefitBar:BaseClass
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public string Img { get; set; }
    }
}
