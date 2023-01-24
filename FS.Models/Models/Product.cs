using FS.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FS.Models.Models
{
    public class Product : BaseClass
    {
        [Key]
        public int ProductId { get; set; }
        //
        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "لطفا نام را وارد کنید")]
        [MaxLength(500)]
        public string Name { get; set; }
        //
        [Display(Name = "قیمت محصول")]
        [Required(ErrorMessage = "لطفا قیمت را وارد کنید")]
        public double Price { get; set; }
        //
        [Display(Name = "تعداد محصول")]
        [Required(ErrorMessage = "لطفا تعداد را وارد کنید")]
        public int Count { get; set; }
        //
        [Display(Name = "توضیحات محصول")]
        [Required(ErrorMessage = "لطفا توضیحات محصول را وارد کنید")]
        public string Description { get; set; }
        //
        [Display(Name = "عکس محصول")]
        //[Required(ErrorMessage = "لطفا عکس محصول را وارد کنید")]
        public string ProductPic { get; set; }
        //
        [Display(Name = "عکس محصول")]
        //[Required(ErrorMessage = "لطفا عکس محصول را وارد کنید")]
        public string ProductPic2 { get; set; }

        //
        [Display(Name = "وضعیت")]
        public bool isVerified { get; set; }
        //
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public UnitToProduct UnitToProduct { get; set; }
    }
}
