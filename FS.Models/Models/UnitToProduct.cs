using FS.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
