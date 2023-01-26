using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class UnitToProduct : BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام واحد")]
        [MaxLength(50)]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }
        //
        [Display(Name = "نام محصول")]
        [MaxLength(50)]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
