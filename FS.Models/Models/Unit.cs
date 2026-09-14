using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class Unit:BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "??????? ????")]
        [Required(ErrorMessage = "???? ???? ?? ???? ????")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "??????? ????")]
        [MaxLength(500)]
        public string Description { get; set; }
    }
}
