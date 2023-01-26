using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class Unit:BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام واحد")]
        [Required(ErrorMessage = "لطفا واحد را وارد کنید")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "توضیحات واحد")]
        [MaxLength(500)]
        public string Description { get; set; }


    }
}
